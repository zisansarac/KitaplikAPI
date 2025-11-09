using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyKitaplikApi.Models.Auth;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // DI: Identity'nin UserManager servisini constructor dan istiyoruz.
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // - REGISTER (KAYIT) - POST /api/Auth/Register -
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // 1. Kullanıcının zaten var olup olmadığını kontrol et
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Hata", Message = "Kullanıcı adı zaten kullanımda" });
        }

        // 2. Yeni IdentityUser nesnesini oluştur
        IdentityUser user = new IdentityUser()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        // 3. Kullanıcıyı veritabanına kaydet (Parola otomatik hash'lenir)
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            // parola kurallarına uymama
            return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Hata", Message = "Kullanıcı kaydı başarısız! Parola gereksinimlerini kontrol edin." });
        }

        return Ok(new { Status = "Başarılı", Message = "Kullanıcı başarıyla oluşturuldu." });

    }



}