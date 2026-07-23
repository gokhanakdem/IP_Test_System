using Microsoft.EntityFrameworkCore;
using IpEnvanterAPI.Models; 

namespace IpEnvanterAPI.Data // Eğer proje adın farklıysa IpEnvanterAPI kısmını değiştir.
{
    // DbContext'ten miras alarak bunun bir "Veritabanı Köprüsü" olduğunu belirtiyoruz.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Cihaz sınıfımızı, SQL'de "Cihazlar" adında bir tabloya dönüştürür.
        public DbSet<Cihaz> Cihazlar { get; set; }
    }
}
