namespace KanGainNET.Models
{
    public class Obecnosc {
        public int Id { get; set; }

        public DateTime Data { get; set; }

        public string Typ { get; set; } = null!;

        public int KartaRFIDId { get; set; }

        public KartaRFID KartaRFID { get; set; } = null!;
    }
}