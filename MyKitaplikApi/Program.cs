// MyKitaplikApi/Program.cs
using MyKitaplikApi.Data;
using MyKitaplikApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Gerekli using'ler: System.IO, MyKitaplikApi.Data, MyKitaplikApi.Services, Microsoft.EntityFrameworkCore

// DBContext ve SQLite Kaydı
builder.Services.AddDbContext<KitaplikDbContext>(options =>
{
    // SQLite bağlantı dizesi
    options.UseSqlite("DataSource=Kitaplik.db");
});

// YENİ EKLEME 1: Identity Ayarları (Kullanıcı, Parola Kuralları)

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
  .AddEntityFrameworkStores<KitaplikDbContext>()
  .AddDefaultTokenProviders();

// paralo ayarlarını basit tutalım:
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false; // rakam zorunlu olmasın
    options.Password.RequireLowercase = false; // küçük harf zorunlu olmasın
    options.Password.RequireUppercase = false; // büyük harf zorunlu olmasın
    options.Password.RequireNonAlphanumeric = false; // sembol zorunlu olmasın
    options.Password.RequiredLength = 4; // min 4 karakter olsun

});

// YENİ EKLEME 2: JWT (Token) Ayarları

var jwtSecret = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // JWT Token'ın kimin tarafından oluşturulduğunu doğrula (bizim api miz olmalı)
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Audience"],

        // JWT Token'ın kimin için oluşturulduğunu doğrula (Frontend'imiz olmalı)
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // token ın süresini doğrula
        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),

         ClockSkew = TimeSpan.Zero // token süresinin bitiminde gecikme olmasın
    };
});

// YENİ EKLEME: KitapService'i DI sistemine kaydet
builder.Services.AddScoped<KitapService>();
// AddScoped yaşam döngüsü seçildi (her HTTP isteği için yeni bir servis)

// CORS ayarları için gerekli değişken adı
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; 

// 1. CORS Servisini DI sistemine ekleme
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
     builder =>
    {
    // **Burası, API'nize erişim izni verdiğiniz yerdir.**
    // Frontend'inizin çalışacağı adresi buraya yazmalıyız.
    // 3000 veya 5173 (React/Vue/Angular'ın varsayılan portları) kullanabilirsiniz.
    builder.WithOrigins("http://localhost:3000", 
     "http://localhost:5173") 
     .AllowAnyHeader()  // Tüm başlıkları kabul et
     .AllowAnyMethod(); // Tüm HTTP metotlarına (GET, POST, DELETE) izin ver
     });
});

// Standart Web API Ayarları
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// YENİ EKLEME 3: HTTP İstek Hattına (Pipeline) Güvenliği Dahil Etme

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
// 2. CORS'u HTTP istek hattına dahil et
// Bu, app.UseAuthorization() satırından ÖNCE gelmelidir.
app.UseCors(MyAllowSpecificOrigins);
// DİKKAT: UseAuthentication, UseAuthorization'dan ÖNCE gelmelidir!
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();