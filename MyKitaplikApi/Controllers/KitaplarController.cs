// MyKitaplikApi/Controllers/KitaplarController.cs
using Microsoft.AspNetCore.Mvc;
using MyKitaplikApi.Models;
using MyKitaplikApi.Services; // Sadece Servis'e bağımlıyız!

[Route("api/[controller]")]
[ApiController]
public class KitaplarController : ControllerBase
{
    private readonly KitapService _kitapService; 

    public KitaplarController(KitapService kitapService) // DI ile Service alınır
    {
        _kitapService = kitapService;
    }

    // GET /api/Kitaplar
    [HttpGet] 
    public async Task<IActionResult> GetirTumKitaplari()
    {
        var kitaplar = await _kitapService.GetAll(); 
        return Ok(kitaplar);
    }

    // GET /api/Kitaplar/2
    [HttpGet("{id}")]
    public async Task<IActionResult> GetirKitapById(int id)
    {
        var kitap = await _kitapService.GetById(id);
        if (kitap == null) return NotFound();
        return Ok(kitap);
    }

    // POST /api/Kitaplar
    [HttpPost]
    public async Task<IActionResult> EkleYeniKitap([FromBody] Kitap yeniKitap)
    {
        await _kitapService.Add(yeniKitap); 
        return CreatedAtAction(nameof(GetirKitapById), new { id = yeniKitap.Id }, yeniKitap); 
    }

    // PUT /api/Kitaplar/2
    [HttpPut("{id}")]
    public async Task<IActionResult> GuncelleKitap(int id, [FromBody] Kitap guncelKitap)
    {
        if (id != guncelKitap.Id) return BadRequest();

        // Service'e güncelleme işlemini yapmasını söyle
        await _kitapService.Update(guncelKitap);
        
        // Hata kontrolü burada eksik, ama şimdilik basit tutalım.
        // KitapService'ten dönen bool değerini kontrol etmek daha doğru olur.

        return NoContent();
    }

    // DELETE /api/Kitaplar/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> SilKitap(int id)
    {
        bool silindi = await _kitapService.Delete(id);
        if (!silindi) return NotFound();
        
        return NoContent();
    }
}