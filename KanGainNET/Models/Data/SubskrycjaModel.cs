namespace KanGainNET.Models
{
    public class Subskrypcja {
        public int Id { get; set; }
        public DateTime DataStartu { get; set; }
        public DateTime DataKonca { get; set; }
        
        public int UzytkownikId { get; set; }
        public virtual Uzytkownik Uzytkownik { get; set; }
        
        public int TypKarnetuId { get; set; }
        public virtual TypKarnetu TypKarnetu { get; set; }
        
        public virtual ICollection<Platnosc> Platnosci { get; set; }
    }
}