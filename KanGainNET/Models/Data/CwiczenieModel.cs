namespace KanGainNET.Models
{
    public class Cwiczenie {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Kategoria { get; set; }
        public string Opis { get; set; }
        public virtual ICollection<PlanTreningowy> PlanyTreningowe { get; set; } = new List<PlanTreningowy>();
    }
}