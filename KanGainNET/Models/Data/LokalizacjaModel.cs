namespace KanGainNET.Models
{
    public class Lokalizacja {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Adres { get; set; }
        public string Miasto { get; set; }
        
        public virtual ICollection<Sala> Sale { get; set; }
        public virtual ICollection<Sprzet> Sprzety { get; set; }
    }
}