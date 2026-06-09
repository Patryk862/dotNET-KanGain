using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanGainNET.Models
{
    public class TypKarnetu
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nazwa { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być dodatnia.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cena { get; set; }

        [Range(1, 3650, ErrorMessage = "Czas trwania musi być dodatni.")]
        public int CzasTrwaniaDni { get; set; }

        public virtual ICollection<Subskrypcja> Subskrypcje { get; set; }
    }
}