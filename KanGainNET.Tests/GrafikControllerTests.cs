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

        [Fact] // Sprawdzenie, czy metoda Zapisz zwraca błąd, gdy sala jest już zajęta w tym czasie
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

        [Fact] // Sprawdzenie, czy metoda Zapisz zwraca błąd, gdy pracownik prowadzi już inne zajęcia w tym czasie
        public async Task Zapisz_ZwracaBlad_GdyPracownikProwadziJuzInneZajeciaWTymCzasie()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();
            var dzisiaj = DateTime.Today;

            dbContext.Sale.Add(new Sala { Id = 1, Nazwa = "Sala A", LokalizacjaId = 1 });
            dbContext.Sale.Add(new Sala { Id = 2, Nazwa = "Sala B", LokalizacjaId = 1 });

            dbContext.Pracownicy.Add(new Pracownik { Id = 1, Specjalizacja = "Joga", UzytkownikId = 1 });

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