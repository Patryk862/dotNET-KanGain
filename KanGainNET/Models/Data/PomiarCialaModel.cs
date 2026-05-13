namespace KanGainNET.Models
{
    public class PomiarCiala {
        public int Id { get; set; }
        public DateTime DataPomiaru { get; set; }
        public double Waga { get; set; }
        public double TkankaTluszczowa { get; set; }
        
        public int UzytkownikId { get; set; }
        public virtual Uzytkownik Uzytkownik { get; set; }
    }
}