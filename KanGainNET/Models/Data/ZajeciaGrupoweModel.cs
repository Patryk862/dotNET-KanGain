namespace KanGainNET.Models
{
    public class ZajeciaGrupowe {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Opis { get; set; }
        public virtual ICollection<Grafik> Grafiki { get; set; }
    }
}