namespace KanGainNET.Models
{
    public class Sala {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public int Pojemnosc { get; set; }
        
        public int LokalizacjaId { get; set; }
        public virtual Lokalizacja Lokalizacja { get; set; }
        
        public virtual ICollection<Grafik> Grafiki { get; set; }
    }
}