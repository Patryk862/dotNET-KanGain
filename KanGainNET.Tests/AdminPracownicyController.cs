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
    public class AdminPracownicyControllerTests
    {
        private SilowniaContext PobierzBazeWypelnionaDanymi()
        {
            var options = new DbContextOptionsBuilder<SilowniaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SilowniaContext(options);
        }

        [Fact] // Sprawdzenie, czy metoda Index poprawnie filtruje użytkowników po nazwisku
        public async Task Index_WyszukiwanieUzytkownikow_FiltrujePoprawniePoNazwisku()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();

            dbContext.Role.Add(new Rola { Id = 2, Nazwa = "Klient" });

            dbContext.Uzytkownicy.Add(new Uzytkownik
            {
                Id = 1,
                Email = "jan@test.pl",
                Haslo = "123",
                RolaId = 2,
                Profil = new ProfilUzytkownika { Imie = "Jan", Nazwisko = "Kowalski", Telefon = "111" }
            });
            dbContext.Uzytkownicy.Add(new Uzytkownik
            {
                Id = 2,
                Email = "adam@test.pl",
                Haslo = "123",
                RolaId = 2,
                Profil = new ProfilUzytkownika { Imie = "Adam", Nazwisko = "Nowak", Telefon = "222" }
            });
            await dbContext.SaveChangesAsync();

            var controller = new AdminPracownicyController(dbContext);

            var result = await controller.Index("kowalski");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Uzytkownik>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal("Jan", model.First().Profil.Imie);
        }

        [Fact] // Sprawdzenie, czy metoda ZmienRole poprawnie awansuje klienta na trenera i tworzy profil trenera z domyślną specjalizacją
        public async Task ZmienRole_AwansNaPracownika_TworzyWpisWTabeliPracownicyZDomyslnaSpecjalizacja()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();

            dbContext.ZajeciaGrupowe.Add(new ZajeciaGrupowe { Id = 1, Nazwa = "Boks", Opis = "Trening bokserski" });

            dbContext.Uzytkownicy.Add(new Uzytkownik { Id = 1, Email = "klient@test.pl", Haslo = "123", RolaId = 2 });
            await dbContext.SaveChangesAsync();

            var controller = new AdminPracownicyController(dbContext);

            await controller.ZmienRole(id: 1, nowaRolaId: 3);

            var awansowanyUser = await dbContext.Uzytkownicy.FindAsync(1);
            var nowyWpisPracownika = await dbContext.Pracownicy.FirstOrDefaultAsync(p => p.UzytkownikId == 1);

            Assert.Equal(3, awansowanyUser.RolaId); 
            Assert.NotNull(nowyWpisPracownika);    
            Assert.Equal("Boks", nowyWpisPracownika.Specjalizacja); 
        }
    }
}