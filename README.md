# 🌐 IP Test & Envanter Sistemi (IP Test System)

Bu proje, yerel ağdaki (LAN) cihazların IP ve MAC adreslerini kayıt altına alan ve tek bir tıklamayla tüm ağdaki cihazların aktif/pasif durumlarını (Ping atarak) anlık olarak kontrol edebilen tam teşekküllü bir IT envanter yönetim sistemidir.

## 🚀 Özellikler

* **Cihaz Kaydı:** Ağdaki cihazların IP adresi, MAC adresi ve cihaz sahibi bilgilerini veritabanına kaydetme.
* **Toplu Ping Testi:** Veritabanındaki tüm IP'lere asenkron olarak ping atıp cihazların ağda aktif olup olmadığını tespit etme.
* **Anlık Durum Takibi:** Cihazların son kontrol edilme saatini ve güncel durumunu listeleme.
* **Dinamik Arayüz:** Sayfayı yenilemeden (Fetch API ile) verileri güncelleyen, kullanıcı dostu kontrol paneli.
* **Monorepo Mimarisi:** Backend (API) ve Frontend (Arayüz) yapılarının aynı proje dizininde, izole klasörlerde yönetilmesi.

## 🛠️ Kullanılan Teknolojiler

**Backend (API):**
* C# / ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* System.Net.NetworkInformation (Ping Sınıfı)
* CORS Politikaları

**Frontend (Arayüz):**
* HTML5 & CSS3
* Vanilla JavaScript (ES6+ / Fetch API)
* Bootstrap 5

## 📂 Proje Klasör Yapısı

Projeyi klonladığınızda iki ana klasör göreceksiniz:
* `IpEnvanterAPI/`: C# ile yazılmış veritabanı ve sunucu kodlarının bulunduğu klasör.
* `Frontend/`: Kullanıcının etkileşime girdiği HTML/CSS/JS arayüz dosyalarının bulunduğu klasör.
* Eğer bir IP veritabanı varsa bunu .csv dosyasına çevirerek sistemin veritabanına ekleyerek Ping atabilirsiniz.
* Eğer ki bu sistemde kaydedilen Json formatlı verileri .csv ye dönüştürerek indirebilirisiniz.

## ⚙️ Kurulum ve Çalıştırma

1. Projeyi bilgisayarınıza indirin (Clone).
2. `IpEnvanterAPI` klasörünü Visual Studio ile açın.
3. `appsettings.json` dosyasından SQL Server bağlantı dizenizi (Connection String) kendi bilgisayarınıza göre güncelleyin.
4. Visual Studio üzerinden projeyi çalıştırın (Swagger API arayüzü açılacaktır).
5. API çalışır durumdayken, `Frontend` klasöründeki `index.html` dosyasını herhangi bir modern tarayıcıda açın.
6. Arayüz üzerinden cihaz ekleyebilir ve "Tümüne Ping At" butonu ile ağı tarayabilirsiniz!

---
*Bu proje, kurumsal ağ yönetimi süreçlerini otomatize etmek ve pratik bir envanter takibi sağlamak amacıyla geliştirilmiştir.*
