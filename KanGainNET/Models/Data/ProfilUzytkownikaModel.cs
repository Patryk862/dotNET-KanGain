namespace KanGainNET.Models
{
    public class ProfilUzytkownika {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Telefon { get; set; }
        
        public int UzytkownikId { get; set; }
        public virtual Uzytkownik Uzytkownik { get; set; }
    }
}