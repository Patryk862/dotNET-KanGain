namespace KanGainNET.Models
{
    public class Pracownik {
        public int Id { get; set; }
        public string Specjalizacja { get; set; }
        public DateTime DataZatrudnienia { get; set; }
        
        public int UzytkownikId { get; set; }
        public virtual Uzytkownik Uzytkownik { get; set; }

        public virtual ICollection<Grafik> Grafiki { get; set; }
        public virtual ICollection<PlanTreningowy> PlanyTreningowe { get; set; }
    }
}