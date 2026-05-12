namespace KanGainNET.Models
{
    public class KartaRFID
    {
        public int Id { get; set; }

        public string UID { get; set; } = null!;

        public bool Aktywna { get; set; } = true;

        public int UzytkownikId { get; set; }

        public Uzytkownik Uzytkownik { get; set; } = null!;
    }
}
