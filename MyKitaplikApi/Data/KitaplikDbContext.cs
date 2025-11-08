using Microsoft.EntityFrameworkCore;
using MyKitaplikApi.Models; //Kitap modelimiz

namespace MyKitaplikApi.Data
{
    // DbContext'ten miras alarak EF Core'a bağlanıyoruz
    public class KitaplikDbContext : DbContext
    {
        // Constructor: Bağlantı ayarlarını DI sisteminden alır.
        public KitaplikDbContext(DbContextOptions<KitaplikDbContext> options) : base(options)
        {
        }

        // DbSet: Veritabanındaki tablolarımıza karşılık gelir. Kitap sınıfı, veritabanında "Kitaplar" adında bir tabloya eşlenecek.
        public DbSet<Kitap> Kitaplar { get; set; }
    } 
}

