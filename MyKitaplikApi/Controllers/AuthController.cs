using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        //1. Kullanıcıyı bul
        var user = await _userManager.FindByNameAsync(model.Username);

        //2. Kullanıcı varsa ve parola doğruysa
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            // 3. Kullanıcı bilgilerini içeren "Claim" listesini oluştur (Bu, token'ın içeriğidir).
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            //4. Toekn'ı oluştur
            var token = GetToken(authClaims);

            //5. Token'ı ve son kullanma tarihini Frontend'e gönder
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo, // Token'ın geçerlilik süresi
                username = user.UserName
            });
        }
        // Başarısız giriş
        return Unauthorized(new { Status = "Hata", Message = "Kullanıcı adı veya parola hatalı!" });
    }

    // Yardımcı Metot: JWT Token Üretimi
    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        // DI ile IConfiguration servisini alın
        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
    
        //JWT ayarlarını Program.cs den çek
        var jwtSecret = config["Jwt:Key"]!;
        var validIssuer = config["Jwt:Issuer"]!;
        var validAudience = config["Jwt:Audience"]!;

        // Gizli anahtarı byte dizisine çevir
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

        // Token detaylarını tanımla
        var token = new JwtSecurityToken(
            issuer: validIssuer,
            audience: validAudience,
            expires: DateTime.Now.AddHours(24), // token 24 saat geçerli olsun
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
     
    }


}