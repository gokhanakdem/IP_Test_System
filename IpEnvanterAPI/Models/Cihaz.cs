using System;

namespace IpEnvanterAPI.Models 
{
    // Bu sınıf, MsSQL'de "Cihazlar" adında bir tabloya dönüşecek
    public class Cihaz
    {
        public int Id { get; set; } // SQL'deki otomatik artan birincil anahtar
        public string IpAdresi { get; set; } // Örn: "192.168.1.50"
        public string MacAdresi { get; set; } // Örn: "00:1A:2B..." (XLog'dan gelebilir)
        public string CihazSahibi { get; set; } // Örn: "Ahmet Yılmaz - İK"
        public bool AktifMi { get; set; } // True = Açık(Yeşil), False = Kapalı(Kırmızı)
        public DateTime SonKontrolTarihi { get; set; } // Ping'in en son atıldığı zaman
    }
}
