using System.ComponentModel.DataAnnotations;

namespace KanGainNET.Models
{
    public class Sala
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa sali jest wymagana.")]
        [MaxLength(100)]
        public string Nazwa { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Pojemność sali musi być większa niż 0.")]
        public int Pojemnosc { get; set; }

        public int LokalizacjaId { get; set; }
        public virtual Lokalizacja Lokalizacja { get; set; }

        public virtual ICollection<Grafik> Grafiki { get; set; }
    }
}