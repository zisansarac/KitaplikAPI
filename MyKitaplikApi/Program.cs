// MyKitaplikApi/Program.cs
using MyKitaplikApi.Data;
using MyKitaplikApi.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Gerekli using'ler: System.IO, MyKitaplikApi.Data, MyKitaplikApi.Services, Microsoft.EntityFrameworkCore

// DBContext ve SQLite Kaydı
builder.Services.AddDbContext<KitaplikDbContext>(options =>
{
    // SQLite bağlantı dizesi
    options.UseSqlite("DataSource=Kitaplik.db"); 
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

var app = builder.Build();

// İstek Hattı (Pipeline) Ayarları
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
// 2. CORS'u HTTP istek hattına dahil et
// Bu, app.UseAuthorization() satırından ÖNCE gelmelidir.
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();
app.MapControllers();

app.Run();