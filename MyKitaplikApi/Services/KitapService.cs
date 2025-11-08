// MyKitaplikApi/Services/KitapService.cs
using MyKitaplikApi.Data;
using MyKitaplikApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyKitaplikApi.Services
{
    // Bu sınıf, İş Mantığı/Veri Erişimini kapsar.
    public class KitapService
    {
        private readonly KitaplikDbContext _context;

        public KitapService(KitaplikDbContext context) // DI: DbContext'i constructor'dan alır
        {
            _context = context;
        }

        public async Task<List<Kitap>> GetAll()
        {
            return await _context.Kitaplar.ToListAsync();
        }

        public async Task<Kitap> GetById(int id)
        {
            return await _context.Kitaplar.FindAsync(id);
        }

        public async Task Add(Kitap kitap)
        {
            // İş Kuralı Kontrolü burada yapılabilir (örn: if (kitap.Baslik.Length < 3) { throw new Exception("...") })
            _context.Kitaplar.Add(kitap);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Kitap guncelKitap)
        {
            _context.Entry(guncelKitap).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap == null) return false;

            _context.Kitaplar.Remove(kitap);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> KitapVarMi(int id)
        {
            return await _context.Kitaplar.AnyAsync(e => e.Id == id);
        }
    }
}