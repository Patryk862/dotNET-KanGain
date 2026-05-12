namespace KanGainNET.Models
{
    public class Platnosc {
        public int Id { get; set; }
        public decimal Kwota { get; set; }
        public DateTime DataPlatnosci { get; set; }
        public string Metoda { get; set; }
        
        public int SubskrypcjaId { get; set; }
        public virtual Subskrypcja Subskrypcja { get; set; }
    }
}