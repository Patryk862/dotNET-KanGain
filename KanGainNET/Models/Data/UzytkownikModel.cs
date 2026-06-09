using System;
using System.Collections.Generic;

namespace KanGainNET.Models
{
    public class Uzytkownik
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Haslo { get; set; }
        public DateTime DataRejestracji { get; set; }

        public string? Specjalizacja { get; set; }
        public DateTime? DataZatrudnienia { get; set; }

        public int RolaId { get; set; }
        public virtual Rola Rola { get; set; }

        public virtual ProfilUzytkownika Profil { get; set; }

        public virtual ICollection<Subskrypcja> Subskrypcje { get; set; }
        public virtual ICollection<Rezerwacja> Rezerwacje { get; set; }

        public virtual ICollection<Grafik> Grafiki { get; set; }
        public virtual ICollection<PlanTreningowy> PlanyTreningoweJakoKlient { get; set; }
        public virtual ICollection<PlanTreningowy> PlanyTreningoweJakoTrener { get; set; }
    }
}