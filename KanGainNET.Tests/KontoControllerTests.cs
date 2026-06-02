using KanGainNET.Controllers;
using KanGainNET.Data;
using KanGainNET.Models;
using KanGainNET.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace KanGainNET.Tests
{
    public class KontoControllerTests 
    {
        private SilowniaContext PobierzBazeWypelnionaDanymi()
        {
            var options = new DbContextOptionsBuilder<SilowniaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SilowniaContext(options);
        }

        [Fact] // Sprawdzenie, czy rejestracja z istniejącym emailem zwraca błąd formularza
        public async Task Rejestracja_GdyEmailIstnieje_ZwracaBladFormularza()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();

            dbContext.Uzytkownicy.Add(new Uzytkownik
            {
                Email = "zajety@email.com",
                Haslo = "StareHaslo123",
                DataRejestracji = DateTime.Now
            });
            await dbContext.SaveChangesAsync();

            var controller = new KontoController(dbContext);

            var formularzRejestracji = new RejestracjaViewModel
            {
                Email = "zajety@email.com", 
                Haslo = "NoweHaslo123",
                Imie = "Andrzej",
                Nazwisko = "Tester"
            };

            var result = await controller.Rejestracja(formularzRejestracji);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.False(controller.ModelState.IsValid);

            Assert.True(controller.ModelState.ContainsKey("Email"));
        }

        [Fact] // Sprawdzenie, czy rejestracja z poprawnymi danymi tworzy użytkownika i przekierowuje do logowania
        public async Task Rejestracja_GdyDanePoprawne_TworzyUzytkownikaIPrzekierowuje()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();
            var controller = new KontoController(dbContext);

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var formularzRejestracji = new RejestracjaViewModel
            {
                Email = "nowy@klient.pl",
                Haslo = "MojeTajneHaslo123",
                Imie = "Pudzian",
                Nazwisko = "Kowalski",
                Telefon = "123456789"
            };

            var result = await controller.Rejestracja(formularzRejestracji);

            var uzytkownicyWBazie = await dbContext.Uzytkownicy.ToListAsync();
            Assert.Single(uzytkownicyWBazie);

            var dodanyUzytkownik = uzytkownicyWBazie.First();
            Assert.Equal("nowy@klient.pl", dodanyUzytkownik.Email);

            Assert.NotEqual("MojeTajneHaslo123", dodanyUzytkownik.Haslo);
            Assert.NotEmpty(dodanyUzytkownik.Haslo);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Logowanie", redirectResult.ActionName);
        }
    }
}