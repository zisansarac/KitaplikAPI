using Microsoft.AspNetCore.Mvc;
using MyKitaplikApi.Models; // Yeni oluşturduğumuz modeli kullanmak için

[Route("api/[controller]")] 
[ApiController]
public class KitaplarController : ControllerBase
{
    // Geçici olarak Veritabanı yerine statik bir liste kullanıyoruz.
    private static List<Kitap> Kitaplar = new List<Kitap>
    {
        new Kitap { Id = 1, Baslik = "Sefiller", Yazar = "Victor Hugo", SayfaSayisi = 1488 },
        new Kitap { Id = 2, Baslik = "Dönüşüm", Yazar = "Franz Kafka", SayfaSayisi = 70 }
    };

    // ---------------------- 1. GET (Okuma) ----------------------

    // Rota: GET /api/Kitaplar
    [HttpGet] 
    public IActionResult GetirTumKitaplari()
    {
        return Ok(Kitaplar); // HTTP 200 OK yanıtı ile tüm listeyi döndür.
    }

    // Rota: GET /api/Kitaplar/2
    [HttpGet("{id}")]
    public IActionResult GetirKitapById(int id)
    {
        var kitap = Kitaplar.FirstOrDefault(k => k.Id == id);
        
        if (kitap == null)
        {
            return NotFound($"ID'si {id} olan kitap bulunamadı."); // HTTP 404 Not Found yanıtı.
        }
        
        return Ok(kitap); // HTTP 200 OK yanıtı ile tek kitabı döndür.
    }

    // ---------------------- 2. POST (Oluşturma) ----------------------

    // Rota: POST /api/Kitaplar
    [HttpPost]
    public IActionResult EkleYeniKitap([FromBody] Kitap yeniKitap)
    {
        // ID'yi manuel olarak atama (gerçek projede DB yapar)
        yeniKitap.Id = Kitaplar.Max(k => k.Id) + 1;
        Kitaplar.Add(yeniKitap);

        // CreatedAtAction: HTTP 201 Created yanıtı döndürür. Yeni kaynağın URL'ini de sağlar.
        return CreatedAtAction(nameof(GetirKitapById), new { id = yeniKitap.Id }, yeniKitap); 
    }
    
    // ---------------------- 3. PUT (Güncelleme) ----------------------

    // Rota: PUT /api/Kitaplar/2
    [HttpPut("{id}")]
    public IActionResult GuncelleKitap(int id, [FromBody] Kitap guncelKitap)
    {
        var kitap = Kitaplar.FirstOrDefault(k => k.Id == id);

        if (kitap == null)
        {
            return NotFound(); // HTTP 404
        }

        // Kitabın özelliklerini güncelle
        kitap.Baslik = guncelKitap.Baslik;
        kitap.Yazar = guncelKitap.Yazar;
        kitap.SayfaSayisi = guncelKitap.SayfaSayisi;
        
        return NoContent(); // HTTP 204 No Content (Başarılı güncelleme, ama yanıt gövdesi boş).
    }

    // ---------------------- 4. DELETE (Silme) ----------------------

    // Rota: DELETE /api/Kitaplar/1
    [HttpDelete("{id}")]
    public IActionResult SilKitap(int id)
    {
        var kitap = Kitaplar.FirstOrDefault(k => k.Id == id);
        
        if (kitap == null)
        {
            return NotFound(); // HTTP 404
        }
        
        Kitaplar.Remove(kitap);
        
        return NoContent(); // HTTP 204 No Content
    }
}