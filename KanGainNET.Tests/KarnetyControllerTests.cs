using KanGainNET.Controllers;
using KanGainNET.Data;
using KanGainNET.Models;
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
    public class KarnetyControllerTests // Sprawdzenie, czy jak klient kupi nowy karnet, to jego stary karnet
                                        // zostanie oznaczony jako nieważny (czyli jego data ważności zostanie ustawiona na aktualną datę)
    {
        private SilowniaContext PobierzBazeWypelnionaDanymi()
        {
            var options = new DbContextOptionsBuilder<SilowniaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SilowniaContext(options);
        }

        [Fact]
        public async Task Sukces_KupnoNowegoKarnetu_PrzedluzaWaznoscIstniejacego()
        {
            var dbContext = PobierzBazeWypelnionaDanymi();

            var typKarnetu = new TypKarnetu { Id = 1, Nazwa = "Miesięczny", Cena = 100, CzasTrwaniaDni = 30 };
            dbContext.TypyKarnetow.Add(typKarnetu);

            var uzytkownik = new Uzytkownik { Id = 1, Email = "aktywny@klient.pl", Haslo = "Haslo123" };
            dbContext.Uzytkownicy.Add(uzytkownik);

            var dataZakonczeniaStaregoKarnetu = DateTime.Now.AddDays(10);
            dbContext.Subskrypcje.Add(new Subskrypcja
            {
                UzytkownikId = 1,
                TypKarnetuId = 1,
                DataStartu = DateTime.Now.AddDays(-20),
                DataKonca = dataZakonczeniaStaregoKarnetu
            });
            await dbContext.SaveChangesAsync();

            var controller = new KarnetyController(dbContext);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "aktywny@klient.pl")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            var result = await controller.Sukces(1); 

            var subskrypcjeKlienta = await dbContext.Subskrypcje.Where(s => s.UzytkownikId == 1).ToListAsync();
            Assert.Equal(2, subskrypcjeKlienta.Count);

            var nowyKarnet = subskrypcjeKlienta.OrderByDescending(s => s.Id).First();

            Assert.Equal(dataZakonczeniaStaregoKarnetu.Date, nowyKarnet.DataStartu.Date);

            var spodziewanaDataKonca = dataZakonczeniaStaregoKarnetu.AddDays(30);
            Assert.Equal(spodziewanaDataKonca.Date, nowyKarnet.DataKonca.Date);

            var platnosci = await dbContext.Platnosci.ToListAsync();
            Assert.Single(platnosci); 
            Assert.Equal(100, platnosci.First().Kwota);
        }
    }
}
// stworzenie fałszywa baze
// dodanie do niej typ karnetu, użytkownika i aktywnej subskrypcji
// stworzenie kontrolera i ustawienie kontekstu użytkownika
// wywołanie akcji Sukces z id typ karnetu
// pobranie subskrypcji klienta z bazy i sprawdzenie, czy jest ich 2
// sprawdzenie, czy data startu nowego karnetu jest równa dacie zakończenia starego karnetu