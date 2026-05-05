namespace KanGainNET.Models
{
    public class Rola {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public virtual ICollection<Uzytkownik> Uzytkownicy { get; set; }
    }
}