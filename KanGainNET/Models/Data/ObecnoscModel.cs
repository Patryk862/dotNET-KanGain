namespace KanGainNET.Models
{
    public class Obecnosc {
        public int Id { get; set; }
        public DateTime DataWejscia { get; set; }
        public DateTime? DataWyjscia { get; set; }
        
        public int UzytkownikId { get; set; }
        public virtual Uzytkownik Uzytkownik { get; set; }
    }
}