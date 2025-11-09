namespace MyKitaplikApi.Models
{
    public class Kitap
    {
        public int Id { get; set; }
        public string Baslik { get; set; }
        public string Yazar { get; set; }
        public int SayfaSayisi { get; set; }
        public string? UserId { get; set; }

    }
}