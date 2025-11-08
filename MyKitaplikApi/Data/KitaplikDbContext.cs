using Microsoft.EntityFrameworkCore;
using MyKitaplikApi.Models;
    // DbContext'ten miras alarak EF Core'a bağlanıyoruz
    namespace MyKitaplikApi.Data
{
    public class KitaplikDbContext : DbContext
    {
        public KitaplikDbContext(DbContextOptions<KitaplikDbContext> options) : base(options)
        {
        }

        // Dbset, veritabanındaki Kitaplar tablosunu temsil eder.
        public DbSet<Kitap> Kitaplar { get; set; } 
    }
}


