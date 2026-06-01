using KanGainNET.Controllers;
using KanGainNET.Data;
using KanGainNET.Models;
using KanGainNET.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace KanGainNET.Tests
{
    public class PracownikControllerTests
    {
        private SilowniaContext PobierzBazeWypelnionaDanymi()
        {
            var options = new DbContextOptionsBuilder<SilowniaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SilowniaContext(options);
        }

        [Fact] // Sprawdzenie czy metoda StworzPlan poprawnie filtruje użytkowników, pokazując
               // tylko klubowiczów (użytkownikow) i ukrywając adminów oraz pracowników
        public async Task StworzPlan_FiltrujeKlubowiczow_UkrywaAdminowIPracownikow()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();

            var rolaAdmin = new Rola { Id = 1, Nazwa = "Admin" };
            var rolaPracownik = new Rola { Id = 3, Nazwa = "Pracownik" };
            var rolaKlient = new Rola { Id = 2, Nazwa = "Klient" };
            dbContext.Role.AddRange(rolaAdmin, rolaPracownik, rolaKlient);

            dbContext.Uzytkownicy.Add(new Uzytkownik { Id = 1, Email = "admin@klub.pl", Haslo = "SilneHaslo", RolaId = 1 });
            dbContext.Uzytkownicy.Add(new Uzytkownik { Id = 2, Email = "trener@klub.pl", Haslo = "SilneHaslo", RolaId = 3, Pracownik = new Pracownik { Specjalizacja = "Joga" } });
            dbContext.Uzytkownicy.Add(new Uzytkownik { Id = 3, Email = "zwykly.klient@klub.pl", Haslo = "SilneHaslo", RolaId = 2, Profil = new ProfilUzytkownika { Imie = "Jan", Nazwisko = "Kowalski", Telefon = "123123123" } });

            await dbContext.SaveChangesAsync();

            var controller = new PracownikController(dbContext);

            var result = await controller.StworzPlan(null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DodajPlanViewModel>(viewResult.Model);

            Assert.Single(model.Klubowicze);
            Assert.Equal("Jan Kowalski", model.Klubowicze.First().Text);
            Assert.Equal("3", model.Klubowicze.First().Value);
        }
    }
}