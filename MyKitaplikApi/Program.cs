// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container (DI Konteynerine servisleri ekleme)
builder.Services.AddControllers(); // Controller'ları kullanacağımızı belirtir.
builder.Services.AddEndpointsApiExplorer(); // Swagger için gerekli ayar.
builder.Services.AddSwaggerGen(); // API dokümantasyonu (Swagger UI) ekler.

var app = builder.Build();

// Configure the HTTP request pipeline (İstek hattını yapılandırma)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Geliştirme ortamında Swagger'ı aç.
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // HTTP isteklerini HTTPS'e yönlendir.
app.UseAuthorization(); // Yetkilendirme middleware'ini ekle.

app.MapControllers(); // Controller'ları rota sistemiyle eşleştir.

app.Run(); // Uygulamayı başlat.