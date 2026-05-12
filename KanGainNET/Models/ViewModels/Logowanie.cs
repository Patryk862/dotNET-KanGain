using System.ComponentModel.DataAnnotations;

namespace KanGainNET.Models.ViewModels
{
    public class LogowanieViewModel
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string Haslo { get; set; }
    }
}