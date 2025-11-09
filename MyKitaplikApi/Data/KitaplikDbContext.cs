using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyKitaplikApi.Models;
    
    namespace MyKitaplikApi.Data
{
    public class KitaplikDbContext : IdentityDbContext<IdentityUser>
    {
        public KitaplikDbContext(DbContextOptions<KitaplikDbContext> options) : base(options)
        {
        }

        // Dbset, veritabanındaki Kitaplar tablosunu temsil eder.
        public DbSet<Kitap> Kitaplar { get; set; } 
    }
}


