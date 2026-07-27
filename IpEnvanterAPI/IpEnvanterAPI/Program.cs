using Microsoft.EntityFrameworkCore;
using IpEnvanterAPI.Data;
using IpEnvanterAPI.Models; // Kendi proje adýn farklýysa (Örn: Ip_test) burayý ona göre düzelt
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);

// --- VERÝTABANI BAÐLANTISINI AKTÝF ETME ---
// appsettings.json içindeki "DefaultConnection" adresini bul ve SQL'e baðlan
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// API test arayüzü (Swagger) için ayarlar
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. ÇATI KAPANMADAN ÖNCE: Cors servisini (kurallarýný) ekliyoruz
builder.Services.AddCors(options =>
{
    options.AddPolicy("IzinVer", policy =>
    {
        policy.AllowAnyOrigin()   // Herhangi bir adresten (bizim html dosyasýndan)
              .AllowAnyHeader()   // Herhangi bir baþlýkla
              .AllowAnyMethod();  // Herhangi bir metodla (GET, POST vs) eriþime izin ver
    });
});

// --- ÝNÞAAT BÝTÝYOR, ÇATI KAPANIYOR ---
var app = builder.Build();

// 2. ÇATI KAPANDIKTAN SONRA: Eklediðimiz servisi devreye sokuyoruz
app.UseCors("IzinVer");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// Dýþarýdan "/api/cihazlar" adresine girildiðinde çalýþacak kod bloðu
app.MapGet("/api/cihazlar", (AppDbContext db) =>
{
    //SELECT * FROM Cihazlar için LINQ sorgusu
    return db.Cihazlar.ToList();

});
// Dýþarýdan "/api/cihazlar" adresine veri gönderildiðinde (POST) çalýþacak kod
app.MapPost("/api/cihazlar", (AppDbContext db, Cihaz YeniCihaz) =>
{
    db.Cihazlar.Add(YeniCihaz);
    db.SaveChanges();
    return Results.Ok("Cihaz baþarýyla eklendi!");

});
//Ping atma iþlemini yaparak IP yi kontrol ediyoruz
app.MapGet("/api/ping/{ip}", (string ip) =>
{
    Ping pingGonderici = new Ping();

    try
    {
        // Hedef IP'ye ping atýyoruz
        PingReply cevap = pingGonderici.Send(ip);

        // Eðer sonuç baþarýlýysa
        if (cevap.Status == IPStatus.Success)
        {
            return Results.Ok("Cihaz Aktif!"); // 'return' eklendi
        }
        else
        {
            // Zaman aþýmý veya ulaþýlamama durumu
            return Results.BadRequest("Cihaz Pasif veya Ulaþýlamýyor!"); // 'return' eklendi
        }
    }
    catch (Exception ex)
    {
        // IP formatý yanlýþsa veya að hatasý varsa buraya düþer
        return Results.BadRequest($"Ping atýlýrken hata oluþtu: {ex.Message}");
    }
});
// CSV'den Toplu Cihaz Yükleme Rotasý
app.MapPost("/api/cihazlar/csv-yukle", async (IFormFile dosya, AppDbContext db) =>
{
    if (dosya == null || dosya.Length == 0)
        return Results.BadRequest("Lütfen geçerli bir CSV dosyasý seçin.");

    using var streamOkuyucu = new StreamReader(dosya.OpenReadStream());

    while (!streamOkuyucu.EndOfStream)
    {
        var satir = await streamOkuyucu.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(satir)) continue;

        // CSV dosyalarý genellikle virgül (,) ile ayrýlýr. 
        // Beklenen format: "192.168.1.50,00:1A:2B:3C:4D:5E,Ahmet Yýlmaz"
        var hucreler = satir.Split(',');

        if (hucreler.Length >= 3)
        {
            var yeniCihaz = new Cihaz
            {
                IpAdresi = hucreler[0].Trim(),
                MacAdresi = hucreler[1].Trim(),
                CihazSahibi = hucreler[2].Trim(),
                AktifMi = false, // Ýlk eklendiðinde durum pasif baþlar
                SonKontrolTarihi = DateTime.Now
            };
            db.Cihazlar.Add(yeniCihaz);
        }
    }
    await db.SaveChangesAsync();
    return Results.Ok("Cihazlar CSV'den baþarýyla aktarýldý!");
}).DisableAntiforgery();

// Veritabanýndaki tüm cihazlarý kontrol eden rota
app.MapPost("/api/ping/hepsini-kontrol-et", (AppDbContext db) =>
{
    // 1. Veritabanýndaki tüm cihazlarý bir listeye alýyoruz
    var cihazlar = db.Cihazlar.ToList();
    Ping pingGonderici = new Ping();

    // 2. Listedeki her bir cihaz için sýrayla iþlem yapýyoruz (Döngü)
    foreach (var cihaz in cihazlar)
    {
        try
        {
            // pingGonderici ile o anki 'cihaz.IpAdresi'ne ping attýk
            PingReply cevap = pingGonderici.Send(cihaz.IpAdresi);

            // cihazýn 'AktifMi' özelliðini true, deðilse (else) false yap.
            if (cevap.Status == IPStatus.Success)
            {
                cihaz.AktifMi = true;
            }
            else
            {
                cihaz.AktifMi = false;
            }
            //Þu ana atýldý zaman
            cihaz.SonKontrolTarihi = DateTime.Now;
        }
        catch
        {
            // Eðer IP adresi hatalýysa veya að çökükse program patlamasýn, cihazý kapalý sayalým.
            cihaz.AktifMi = false;
            cihaz.SonKontrolTarihi = DateTime.Now;
        }

    }

    // güncellenen tüm cihazlarý veritabanýna kaydettik.
    db.SaveChanges();

    // Ýþlem bitince güncel listeyi kullanýcýya geri gönderiyoruz
    return Results.Ok(cihazlar);
});
// Cihazý veritabanýndan silen rota
app.MapDelete("/api/cihazlar/{id}", (AppDbContext db, int id) =>
{
    var cihaz = db.Cihazlar.Find(id);
    if (cihaz == null)
    {
        return Results.NotFound("Cihaz bulunamadý!");
    }

    db.Cihazlar.Remove(cihaz);
    db.SaveChanges();
    return Results.Ok("Cihaz baþarýyla silindi.");
});

app.Run();

