namespace KanGainNET.Models
{
    public class Sprzet {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string StanTechniczny { get; set; }
        
        public int LokalizacjaId { get; set; }
        public virtual Lokalizacja Lokalizacja { get; set; }
    }
}