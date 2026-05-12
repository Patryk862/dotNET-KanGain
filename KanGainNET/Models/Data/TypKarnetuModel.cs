namespace KanGainNET.Models
{
    public class TypKarnetu {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public decimal Cena { get; set; }
        public int CzasTrwaniaDni { get; set; }
        public virtual ICollection<Subskrypcja> Subskrypcje { get; set; }
    }
}