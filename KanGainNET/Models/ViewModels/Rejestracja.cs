using System.ComponentModel.DataAnnotations;

namespace KanGainNET.Models.ViewModels
{
    public class RejestracjaViewModel
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [MinLength(8, ErrorMessage = "Hasło musi mieć minimum 8 znaków.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", 
            ErrorMessage = "Hasło musi zawierać wielką literę, małą literę, cyfrę i znak specjalny.")]
        [DataType(DataType.Password)]
        public string Haslo { get; set; }

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
        [DataType(DataType.Password)]
        [Compare("Haslo", ErrorMessage = "Hasła nie są identyczne.")]
        public string PotwierdzHaslo { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Imię musi mieć od 2 do 50 znaków.")]
        public string Imie { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwisko musi mieć od 2 do 50 znaków.")]
        public string Nazwisko { get; set; }

        [Phone(ErrorMessage = "Niepoprawny format numeru telefonu.")]
        [RegularExpression(@"^([0-9]{9})$", ErrorMessage = "Podaj 9-cyfrowy numer telefonu bez spacji.")]
        public string Telefon { get; set; }
    }
}