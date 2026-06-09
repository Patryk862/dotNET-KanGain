using KanGainNET.Controllers;
using KanGainNET.Data;
using KanGainNET.Models;
using KanGainNET.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace KanGainNET.Tests
{
    public class GrafikControllerTests
    {
        private SilowniaContext PobierzBazeWypelnionaDanymi()
        {
            var options = new DbContextOptionsBuilder<SilowniaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SilowniaContext(options);
        }

        [Fact]
        public async Task Zapisz_ZwracaBlad_GdySalaJestJuzZajetaWTymCzasie()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();

            dbContext.Sale.Add(new Sala { Id = 1, Nazwa = "Sala Główna", LokalizacjaId = 1 });

            var dzisiaj = DateTime.Today;
            dbContext.Grafiki.Add(new Grafik
            {
                Id = 1,
                SalaId = 1,
                DataStart = dzisiaj.AddHours(10),
                DataKoniec = dzisiaj.AddHours(11)
            });
            await dbContext.SaveChangesAsync();

            var controller = new GrafikController(dbContext);

            var formularz = new GrafikViewModel
            {
                SalaId = 1,
                DataStart = dzisiaj.AddHours(10).AddMinutes(30),
                DataKoniec = dzisiaj.AddHours(11).AddMinutes(30)
            };

            var result = await controller.Zapisz(formularz);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Zapisz_ZwracaBlad_GdyPracownikProwadziJuzInneZajeciaWTymCzasie()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();
            var dzisiaj = DateTime.Today;

            dbContext.Sale.Add(new Sala { Id = 1, Nazwa = "Sala A", LokalizacjaId = 1 });
            dbContext.Sale.Add(new Sala { Id = 2, Nazwa = "Sala B", LokalizacjaId = 1 });

            // Dodajemy trenera bezposrednio do tabeli Uzytkownicy
            dbContext.Uzytkownicy.Add(new Uzytkownik { Id = 1, Email = "trener@test.pl", Haslo = "123", RolaId = 3, Specjalizacja = "Joga" });

            dbContext.Grafiki.Add(new Grafik
            {
                Id = 1,
                SalaId = 1,
                PracownikId = 1,
                DataStart = dzisiaj.AddHours(10),
                DataKoniec = dzisiaj.AddHours(11)
            });
            await dbContext.SaveChangesAsync();

            var controller = new GrafikController(dbContext);

            var formularz = new GrafikViewModel
            {
                SalaId = 2,
                PracownikId = 1,
                DataStart = dzisiaj.AddHours(10).AddMinutes(30),
                DataKoniec = dzisiaj.AddHours(11).AddMinutes(30)
            };

            var result = await controller.Zapisz(formularz);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.False(controller.ModelState.IsValid);
        }
    }
}