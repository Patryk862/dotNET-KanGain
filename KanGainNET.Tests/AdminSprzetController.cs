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
    public class AdminSprzetControllerTests
    {
        private SilowniaContext PobierzBazeWypelnionaDanymi()
        {
            var options = new DbContextOptionsBuilder<SilowniaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SilowniaContext(options);
        }

        [Fact] // Sprawdzenie, czy metoda Index poprawnie filtruje sprzęt na podstawie wybranej lokalizacji
        public async Task Index_GdyPodanoIdLokalizacji_FiltrujeSprzetTylkoDlaTejLokalizacji()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();

            dbContext.Lokalizacje.Add(new Lokalizacja { Id = 1, Nazwa = "Siłownia Śródmieście", Adres = "Kwiatowa 1", Miasto = "Warszawa" });
            dbContext.Lokalizacje.Add(new Lokalizacja { Id = 2, Nazwa = "Siłownia Północ", Adres = "Długa 5", Miasto = "Kraków" });

            dbContext.Sprzet.Add(new Sprzet { Id = 1, Nazwa = "Ławka płaska", LokalizacjaId = 1, StanTechniczny = "Sprawny" });
            dbContext.Sprzet.Add(new Sprzet { Id = 2, Nazwa = "Suwnica Smitha", LokalizacjaId = 2, StanTechniczny = "Sprawny" });
            dbContext.Sprzet.Add(new Sprzet { Id = 3, Nazwa = "Modlitewnik", LokalizacjaId = 2, StanTechniczny = "Sprawny" });
            await dbContext.SaveChangesAsync();

            var controller = new AdminSprzetController(dbContext);

            var result = await controller.Index(2);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Sprzet>>(viewResult.Model);

            Assert.Equal(2, model.Count());
            Assert.DoesNotContain(model, s => s.Id == 1);
        }

        [Fact] // Sprawdzenie , czy metoda ZmienStan poprawnie aktualizuje stan techniczny sprzętu i przekierowuje z zachowaniem lokalizacji
        public async Task ZmienStan_GdyPodanoPoprawneDane_AktualizujeStanWBazieIPrzekierowuje()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();

            dbContext.Sprzet.Add(new Sprzet { Id = 1, Nazwa = "Orbitrek", LokalizacjaId = 5, StanTechniczny = "Sprawny" });
            await dbContext.SaveChangesAsync();

            var controller = new AdminSprzetController(dbContext);

            var result = await controller.ZmienStan(1, "W naprawie");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(5, redirectResult.RouteValues["lokalizacjaId"]);

            var zmienionySprzet = await dbContext.Sprzet.FindAsync(1);
            Assert.Equal("W naprawie", zmienionySprzet.StanTechniczny);
        }
    }
}