using Microsoft.EntityFrameworkCore;
using IpEnvanterAPI.Data;
using IpEnvanterAPI.Models; // Kendi proje adýn farklýysa (Örn: Ip_test) burayý ona göre düzelt

var builder = WebApplication.CreateBuilder(args);

// --- VERÝTABANI BAÐLANTISINI AKTÝF ETME ---
// appsettings.json içindeki "DefaultConnection" adresini bul ve SQL'e baðlan
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// API test arayüzü (Swagger) için ayarlar
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.Run();

