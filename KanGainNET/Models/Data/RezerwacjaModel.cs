namespace KanGainNET.Models
{
    public class Rezerwacja {
        public int Id { get; set; }
        public string Status { get; set; }
        
        public int UzytkownikId { get; set; }
        public virtual Uzytkownik Uzytkownik { get; set; }
        
        public int GrafikId { get; set; }
        public virtual Grafik Grafik { get; set; }
    }
}