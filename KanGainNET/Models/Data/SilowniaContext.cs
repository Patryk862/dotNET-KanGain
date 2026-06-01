using Microsoft.EntityFrameworkCore;
using KanGainNET.Models;
using System.Linq;

namespace KanGainNET.Data
{
    public class SilowniaContext : DbContext
    {
        public SilowniaContext(DbContextOptions<SilowniaContext> options) : base(options) { }

        public DbSet<Uzytkownik> Uzytkownicy { get; set; }
        public DbSet<Rola> Role { get; set; }
        public DbSet<ProfilUzytkownika> ProfileUzytkownikow { get; set; }
        public DbSet<Pracownik> Pracownicy { get; set; }
        public DbSet<TypKarnetu> TypyKarnetow { get; set; }
        public DbSet<Subskrypcja> Subskrypcje { get; set; }
        public DbSet<Platnosc> Platnosci { get; set; }
        public DbSet<Rabat> Rabaty { get; set; }
        public DbSet<Lokalizacja> Lokalizacje { get; set; }
        public DbSet<Sala> Sale { get; set; }
        public DbSet<ZajeciaGrupowe> ZajeciaGrupowe { get; set; }
        public DbSet<Grafik> Grafiki { get; set; }
        public DbSet<Rezerwacja> Rezerwacje { get; set; }
        public DbSet<Cwiczenie> Cwiczenia { get; set; }
        public DbSet<PlanTreningowy> PlanyTreningowe { get; set; }
        public DbSet<PomiarCiala> PomiaryCiala { get; set; }
        public DbSet<KartaRFID> KartyRFID { get; set; }
        public DbSet<Obecnosc> Obecnosci { get; set; }
        public DbSet<Sprzet> Sprzet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definicja precyzji dla typów finansowych
            modelBuilder.Entity<Platnosc>().Property(p => p.Kwota).HasPrecision(18, 2);
            modelBuilder.Entity<TypKarnetu>().Property(t => t.Cena).HasPrecision(18, 2);

            // Definicje relacji 1:1
            modelBuilder.Entity<Uzytkownik>()
                .HasOne(u => u.Profil)
                .WithOne(p => p.Uzytkownik)
                .HasForeignKey<ProfilUzytkownika>(p => p.UzytkownikId);

            modelBuilder.Entity<Uzytkownik>()
                .HasOne(u => u.Pracownik)
                .WithOne(p => p.Uzytkownik)
                .HasForeignKey<Pracownik>(p => p.UzytkownikId);

            modelBuilder.Entity<PlanTreningowy>()
                .HasMany(p => p.Cwiczenia)
                .WithMany(c => c.PlanyTreningowe);

            // Globalne wyłączenie CASCADE DELETE
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}