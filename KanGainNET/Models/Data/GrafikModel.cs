using System;
using System.Collections.Generic;
namespace KanGainNET.Models
{
    public class Grafik
    {
        public int Id { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataKoniec { get; set; }
        
        public int SalaId { get; set; }
        public virtual Sala Sala { get; set; }

        public int? ZajeciaGrupoweId { get; set; }
        public virtual ZajeciaGrupowe ZajeciaGrupowe { get; set; }

        public int? PracownikId { get; set; }
        public virtual Pracownik Pracownik { get; set; }

        public virtual ICollection<Rezerwacja> Rezerwacje { get; set; }
    }
}