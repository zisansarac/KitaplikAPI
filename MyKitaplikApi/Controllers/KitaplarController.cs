using Microsoft.AspNetCore.Mvc;
using MyKitaplikApi.Data; // DbContext'i kullanmak için
using MyKitaplikApi.Models; // Modeli kullanmak için
using Microsoft.EntityFrameworkCore; // ToListAsync() gibi EF metotları için

[Route("api/[controller]")]
[ApiController]
public class KitaplarController : ControllerBase
{
    // 1. DI: DbContext'i constructor aracılığıyla istiyoruz.
    private readonly KitaplikDbContext _context;

    public KitaplarController(KitaplikDbContext context) 
    {
        // Bu _context, Program.cs'te kaydettiğimiz veritabanı bağlantısını temsil eder.
        _context = context; 
    }

    // ---------------------- 1. GET (Okuma) ----------------------

    // Rota: GET /api/Kitaplar
    [HttpGet] 
    public async Task<IActionResult> GetirTumKitaplari()
    {
        // STATİK LİSTE YERİNE: Veritabanından tüm kitapları asenkron çeker.
        var kitaplar = await _context.Kitaplar.ToListAsync(); 
        return Ok(kitaplar);
    }

    // Rota: GET /api/Kitaplar/2
    [HttpGet("{id}")]
    public async Task<IActionResult> GetirKitapById(int id)
    {
        // Statik liste yerine: Veritabanından ID ile bulur.
        var kitap = await _context.Kitaplar.FindAsync(id); 
        
        if (kitap == null)
        {
            return NotFound(); 
        }
        
        return Ok(kitap);
    }

    // ---------------------- 2. POST (Oluşturma) ----------------------

    // Rota: POST /api/Kitaplar
    [HttpPost]
    public async Task<IActionResult> EkleYeniKitap([FromBody] Kitap yeniKitap)
    {
        // Statik listeye ekleme yerine: DbContext'e ekler.
        _context.Kitaplar.Add(yeniKitap);
        
        // KRİTİK ADIM: Değişiklikleri veritabanına kalıcı olarak kaydet!
        await _context.SaveChangesAsync(); 

        // Yeni ID'yi otomatik olarak yeniKitap.Id'ye atar.
        return CreatedAtAction(nameof(GetirKitapById), new { id = yeniKitap.Id }, yeniKitap); 
    }
    
    // ---------------------- 3. PUT (Güncelleme) ----------------------

    [HttpPut("{id}")]
    public async Task<IActionResult> GuncelleKitap(int id, [FromBody] Kitap guncelKitap)
    {
        if (id != guncelKitap.Id) return BadRequest();

        // EF Core'a nesnenin durumunun değiştirildiğini bildir.
        _context.Entry(guncelKitap).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Kitaplar.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw; 
        }
        return NoContent(); 
    }

    // ---------------------- 4. DELETE (Silme) ----------------------

    [HttpDelete("{id}")]
    public async Task<IActionResult> SilKitap(int id)
    {
        var kitap = await _context.Kitaplar.FindAsync(id);
        
        if (kitap == null) return NotFound();
        
        _context.Kitaplar.Remove(kitap);
        await _context.SaveChangesAsync(); 
        
        return NoContent();
    }
}