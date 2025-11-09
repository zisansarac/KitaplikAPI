// MyKitaplikApi/Controllers/KitaplarController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<IActionResult> EkleYeniKitap([FromBody] Kitap yeniKitap)
    {
        // Token içinden yetkili kullanıcının ID'sini çekme (Bu, IDENTITY veritabanındaki ID'dir).
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
        if (currentUserId == null) 
           return Unauthorized(new { Message = "Token geçerli ancak kullanıcı ID'si bulunamadı." });

        yeniKitap.UserId = currentUserId; // Kitaba sahibi atandı!
    
        await _kitapService.Add(yeniKitap); 
        return CreatedAtAction(nameof(GetirKitapById), new { id = yeniKitap.Id }, yeniKitap); 
    }

    // PUT /api/Kitaplar/2
    [HttpPut("{id}")]
    [Authorize]
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
[Authorize] 
public async Task<IActionResult> SilKitap(int id)
{
    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    // 1. Kitabı veritabanından çek (sahibini kontrol etmek için)
    var kitap = await _kitapService.GetById(id);
    
    if (kitap == null) 
        return NotFound();

    // 2. YETKİ KONTROLÜ: Kitabın UserId'si, istek yapanın ID'si ile eşleşiyor mu?
    if (kitap.UserId != currentUserId)
    {
        // Güvenlik Başarısız! Kullanıcı başkasının kitabını silmeye çalışıyor.
        return Forbid(); // HTTP 403 Forbidden yanıtı döndürülür.
    }
    
    // 3. Kontrol başarılı: Silme işlemini yap.
    await _kitapService.Delete(id); 
    
    return NoContent();
}
}