namespace KanGainNET.Models
{
    public class PlanTreningowy {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public DateTime DataStworzenia { get; set; }
        
        public int UzytkownikId { get; set; }
        public virtual Uzytkownik Uzytkownik { get; set; }
        
        public int PracownikId { get; set; }
        public virtual Pracownik Trener { get; set; }
    }
}