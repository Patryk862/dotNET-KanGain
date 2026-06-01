using KanGainNET.Controllers;
using KanGainNET.Data;
using KanGainNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KanGainNET.Tests
{
    public class AdminRaportyControllerTests
    {
        private SilowniaContext PobierzBazeWypelnionaDanymi()
        {
            var options = new DbContextOptionsBuilder<SilowniaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SilowniaContext(options);
        }

        [Fact] // Sprawdzenie , czy metoda Index poprawnie oblicza statystyki i zwraca posortowane płatności
        public async Task Index_ObliczaPoprawneStatystyki_OrazWyswietlaPosortowanePlatnosci()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();
            var dzisiaj = DateTime.Now;

            dbContext.TypyKarnetow.Add(new TypKarnetu { Id = 1, Nazwa = "Karnet Open", Cena = 100, CzasTrwaniaDni = 30 });

            dbContext.Uzytkownicy.Add(new Uzytkownik
            {
                Id = 1,
                Email = "klient@gym.pl",
                Haslo = "123",
                Profil = new ProfilUzytkownika { Imie = "Anna", Nazwisko = "Nowak", Telefon = "987654321" }
            });

            dbContext.Subskrypcje.Add(new Subskrypcja { Id = 1, UzytkownikId = 1, TypKarnetuId = 1, DataKonca = dzisiaj.AddDays(10) });

            dbContext.Subskrypcje.Add(new Subskrypcja { Id = 2, UzytkownikId = 1, TypKarnetuId = 1, DataKonca = dzisiaj.AddDays(-5) });

            dbContext.Platnosci.Add(new Platnosc { Id = 1, SubskrypcjaId = 1, Kwota = 100m, DataPlatnosci = dzisiaj, Metoda = "Karta" });

            dbContext.Platnosci.Add(new Platnosc { Id = 2, SubskrypcjaId = 2, Kwota = 50m, DataPlatnosci = dzisiaj.AddMonths(-1), Metoda = "Gotówka" });

            await dbContext.SaveChangesAsync();

            var controller = new AdminRaportyController(dbContext);
            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(150m, controller.ViewBag.TotalRevenue);

            Assert.Equal(1, controller.ViewBag.ActivePasses);

            Assert.Equal(1, controller.ViewBag.SalesThisMonth);

            var model = Assert.IsAssignableFrom<IEnumerable<Platnosc>>(viewResult.Model);

            Assert.Equal(2, model.Count());

            Assert.Equal(100m, model.First().Kwota);

            Assert.Equal(50m, model.Last().Kwota);
        }
    }
}