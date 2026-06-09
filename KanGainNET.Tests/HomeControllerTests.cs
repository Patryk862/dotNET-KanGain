using KanGainNET.Controllers;
using KanGainNET.Data;
using KanGainNET.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace KanGainNET.Tests
{
    public class HomeControllerTests
    {
        private SilowniaContext PobierzBazeWypelnionaDanymi()
        {
            var options = new DbContextOptionsBuilder<SilowniaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SilowniaContext(options);
        }

        [Fact]
        public async Task MojePlany_ZwracaRedirect_GdyBrakIdZalogowanegoUzytkownika()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();
            var mockLogger = new Mock<ILogger<HomeController>>();

            var controller = new HomeController(mockLogger.Object, dbContext);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity());

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            var result = await controller.MojePlany();

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

        [Fact]
        public async Task MojePlany_ZwracaTylkoPlanyZalogowanegoUzytkownika_PosortowaneDatami()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();
            var mockLogger = new Mock<ILogger<HomeController>>();

            // Dodajemy trenera bezpośrednio do Uzytkownicy
            dbContext.Uzytkownicy.Add(new Uzytkownik { Id = 99, Email = "trener@klub.pl", Haslo = "123", RolaId = 3, Specjalizacja = "Joga" });

            dbContext.PlanyTreningowe.Add(new PlanTreningowy
            {
                Id = 1,
                Nazwa = "Stary plan Klienta 1",
                UzytkownikId = 1,
                TrenerId = 99, // Podmieniono PracownikId na TrenerId
                DataStworzenia = DateTime.Now.AddDays(-10)
            });
            dbContext.PlanyTreningowe.Add(new PlanTreningowy
            {
                Id = 2,
                Nazwa = "Nowy plan Klienta 1",
                UzytkownikId = 1,
                TrenerId = 99,
                DataStworzenia = DateTime.Now
            });

            dbContext.PlanyTreningowe.Add(new PlanTreningowy
            {
                Id = 3,
                Nazwa = "Plan Klienta 2",
                UzytkownikId = 2,
                TrenerId = 99,
                DataStworzenia = DateTime.Now
            });

            await dbContext.SaveChangesAsync();

            var controller = new HomeController(mockLogger.Object, dbContext);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            var result = await controller.MojePlany();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<PlanTreningowy>>(viewResult.Model);

            Assert.Equal(2, model.Count);

            Assert.DoesNotContain(model, p => p.UzytkownikId == 2);

            Assert.Equal("Nowy plan Klienta 1", model.First().Nazwa);
            Assert.Equal("Stary plan Klienta 1", model.Last().Nazwa);
        }
    }
}