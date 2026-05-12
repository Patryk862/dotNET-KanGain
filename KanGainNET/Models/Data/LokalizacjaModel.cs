using System.ComponentModel.DataAnnotations.Schema;

namespace KanGainNET.Models
{
    public class Lokalizacja {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Adres { get; set; }
        public string Miasto { get; set; }
        [Column(TypeName = "decimal(12, 8)")]
        public decimal? Szerokosc { get; set; }
        [Column(TypeName = "decimal(12, 8)")]
        public decimal? Dlugosc { get; set; }

        public virtual ICollection<Sala> Sale { get; set; }
        public virtual ICollection<Sprzet> Sprzety { get; set; }
    }
}