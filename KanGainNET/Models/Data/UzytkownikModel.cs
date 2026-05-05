namespace KanGainNET.Models
{
    public class Uzytkownik {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Haslo { get; set; }
        public DateTime DataRejestracji { get; set; }
        
        public int RolaId { get; set; }
        public virtual Rola Rola { get; set; } // Relacja N:1
        
        public virtual ProfilUzytkownika Profil { get; set; } // Relacja 1:1
        public virtual Pracownik Pracownik { get; set; } // Relacja 1:1

        public virtual ICollection<Subskrypcja> Subskrypcje { get; set; }
        public virtual ICollection<Rezerwacja> Rezerwacje { get; set; }
        public virtual ICollection<Obecnosc> Obecnosci { get; set; }
    }
}