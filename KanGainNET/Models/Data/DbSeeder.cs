using Bogus;
using KanGainNET.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KanGainNET.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(SilowniaContext context)
        {
            Randomizer.Seed = new Random(2026);
            var locale = "pl";
            var faker = new Faker(locale);

            string suroweHaslo = "KanGain2026!";
            string zahaszowaneHaslo = HaszujHaslo(suroweHaslo);

            // --- 1. NAPRAWA ISTNIEJĄCYCH KONT (HASZOWANIE) ---
            var kontaDoNaprawy = await context.Uzytkownicy
                .Where(u => u.Haslo == "KanGain2026!")
                .ToListAsync();

            if (kontaDoNaprawy.Any())
            {
                foreach (var user in kontaDoNaprawy)
                {
                    user.Haslo = zahaszowaneHaslo;
                }
                await context.SaveChangesAsync();
            }

            // ======================================================================
            // 2. REGENERACJA I INTEGRACJA: PEŁNA MATRYCA 6 KARNETÓW Z OBRAZ_2.JPG
            // ======================================================================
            var pelnySlownikKarnetow = new List<TypKarnetu>
            {
                new TypKarnetu { Nazwa = "Miesięczny", Cena = 100.00m, CzasTrwaniaDni = 30 },
                new TypKarnetu { Nazwa = "Półroczny", Cena = 500.00m, CzasTrwaniaDni = 180 },
                new TypKarnetu { Nazwa = "Kwartalny", Cena = 270.00m, CzasTrwaniaDni = 90 },
                new TypKarnetu { Nazwa = "Student 30 Dni", Cena = 99.99m, CzasTrwaniaDni = 30 },
                new TypKarnetu { Nazwa = "Open 30 Dni", Cena = 129.99m, CzasTrwaniaDni = 30 },
                new TypKarnetu { Nazwa = "Premium Rok", Cena = 1199.00m, CzasTrwaniaDni = 365 }
            };

            var obecneKarnetyWDatabase = await context.TypyKarnetow.Select(t => t.Nazwa).ToListAsync();

            foreach (var k in pelnySlownikKarnetow)
            {
                // Jeśli baza nie ma któregoś z tych 6 planów (np. tych co usunąłem), to go grzecznie dopisujemy
                if (!obecneKarnetyWDatabase.Contains(k.Nazwa))
                {
                    context.TypyKarnetow.Add(k);
                }
            }
            await context.SaveChangesAsync();

            // Pobieramy kompletną listę 6 połączonych planów do generowania fejkowej sprzedaży
            var wszystkieTypyKarnetow = await context.TypyKarnetow.ToListAsync();
            // ======================================================================

            // --- 3. POBIERANIE SEKCJI Z BAZY ---
            var wszystkieZajeciaGrupowe = await context.ZajeciaGrupowe.ToListAsync();
            var dostepneSpecjalizacje = wszystkieZajeciaGrupowe.Select(z => z.Nazwa).ToArray();

            if (!dostepneSpecjalizacje.Any())
            {
                dostepneSpecjalizacje = new[] { "Joga", "Box", "Pilates", "Zumba", "CrossFit", "Silownia" };
            }

            var rolaUzytkownik = await context.Role.FirstOrDefaultAsync(r => r.Nazwa == "Uzytkownik" || r.Nazwa == "Klient");
            var rolaTrener = await context.Role.FirstOrDefaultAsync(r => r.Nazwa == "Trener" || r.Nazwa == "Pracownik");

            int idRoliUzytkownik = rolaUzytkownik?.Id ?? 2;
            int idRoliTrener = rolaTrener?.Id ?? 3;

            // --- 4. DOPISYWANIE POTĘŻNEJ BAZY ĆWICZEŃ ---
            var masowyZestawCwiczen = PrzygotujMaseCwiczen();
            var obecneCwiczenia = await context.Cwiczenia.Select(c => c.Nazwa).ToListAsync();
            var cwiczeniaDoDodania = masowyZestawCwiczen.Where(c => !obecneCwiczenia.Contains(c.Nazwa)).ToList();
            if (cwiczeniaDoDodania.Any())
            {
                context.Cwiczenia.AddRange(cwiczeniaDoDodania);
                await context.SaveChangesAsync();
            }

            // --- 5. DOPISYWANIE GLOBALNEJ SIATKI RÓWNYCH 150 LOKALIZACJI ---
            var siatkaSwiat = PobierzSiatkeKlubowSwiat150();
            var obecneLokalizacjeNazwy = await context.Lokalizacje.Select(l => l.Nazwa).ToListAsync();
            var noweLokalizacjeEntities = new List<Lokalizacja>();

            foreach (var klub in siatkaSwiat)
            {
                if (!obecneLokalizacjeNazwy.Contains(klub.Nazwa))
                {
                    noweLokalizacjeEntities.Add(new Lokalizacja
                    {
                        Nazwa = klub.Nazwa,
                        Adres = klub.Adres,
                        Miasto = klub.Miasto,
                        Szerokosc = (decimal)klub.Lat,
                        Dlugosc = (decimal)klub.Lng
                    });
                }
            }
            if (noweLokalizacjeEntities.Any())
            {
                context.Lokalizacje.AddRange(noweLokalizacjeEntities);
                await context.SaveChangesAsync();
            }

            var wszystkieLokalizacjeZDatabase = await context.Lokalizacje.ToListAsync();

            // --- 6. DOPISYWANIE SAL WEDŁUG SZABLONU Z OBRAZ_30.PNG DLA KAŻDEGO MIASTA ---
            var szablonSalZObrazka = new[] {
                "Sala Cardio",
                "Sala Silowa",
                "Sala Zajec Grupowych",
                "Sala Joga",
                "Sala Crossfit",
                "Sala Pilates",
                "Sala Boksu"
            };

            var noweSaleDoDodania = new List<Sala>();
            var obecneSaleZDatabase = await context.Sale.ToListAsync();

            foreach (var lok in wszystkieLokalizacjeZDatabase)
            {
                foreach (var nazwaSali in szablonSalZObrazka)
                {
                    bool juzIstnieje = obecneSaleZDatabase.Any(s => s.LokalizacjaId == lok.Id && s.Nazwa.Equals(nazwaSali, StringComparison.OrdinalIgnoreCase));
                    if (!juzIstnieje)
                    {
                        noweSaleDoDodania.Add(new Sala
                        {
                            Nazwa = nazwaSali,
                            Pojemnosc = nazwaSali.Contains("Silowa") || nazwaSali.Contains("Cardio") ? 50 : 20,
                            LokalizacjaId = lok.Id
                        });
                    }
                }
            }

            if (noweSaleDoDodania.Any())
            {
                context.Sale.AddRange(noweSaleDoDodania);
                await context.SaveChangesAsync();
                obecneSaleZDatabase = await context.Sale.ToListAsync();
            }

            // --- 7. UZUPEŁNIANIE SPRZĘTU ---
            var nowySprzet = new List<Sprzet>();
            var nazwySprzetuSzablon = PobierzSzablonSprzetuZObrazka();

            foreach (var lok in wszystkieLokalizacjeZDatabase)
            {
                if (!await context.Sprzet.AnyAsync(s => s.LokalizacjaId == lok.Id))
                {
                    foreach (var nazwaSprzetu in nazwySprzetuSzablon)
                    {
                        string stan = faker.Random.Bool(0.85f) ? "Dobry" : faker.PickRandom(new[] { "Uszkodzony", "Wymaga serwisu" });
                        nowySprzet.Add(new Sprzet { Nazwa = nazwaSprzetu, StanTechniczny = stan, LokalizacjaId = lok.Id });
                    }
                }
            }

            if (nowySprzet.Any())
            {
                foreach (var chunk in nowySprzet.Chunk(500))
                {
                    context.Sprzet.AddRange(chunk);
                    await context.SaveChangesAsync();
                }
            }

            // --- 8. UZUPEŁNIANIE TRENERÓW DO LICZBY 300 ---
            int obecniTrenerzyKonta = await context.Uzytkownicy.CountAsync(u => u.RolaId == idRoliTrener);
            int iluTrenerowDopisac = 300 - obecniTrenerzyKonta;

            if (iluTrenerowDopisac > 0)
            {
                var nowiTrenerzyUzytkownicy = new List<Uzytkownik>();

                if (!await context.Uzytkownicy.AnyAsync(u => u.Email == "trener.test@kangain.com"))
                {
                    nowiTrenerzyUzytkownicy.Add(new Uzytkownik
                    {
                        Email = "trener.test@kangain.com",
                        Haslo = zahaszowaneHaslo,
                        DataRejestracji = DateTime.Now,
                        RolaId = idRoliTrener,
                        Profil = new ProfilUzytkownika { Imie = "Jan", Nazwisko = "Trener", Telefon = "123456789" }
                    });
                    iluTrenerowDopisac--;
                }

                for (int i = 0; i < iluTrenerowDopisac; i++)
                {
                    var imie = faker.Name.FirstName();
                    var nazwisko = faker.Name.LastName();
                    var email = $"trener.{faker.Internet.UserName(imie, nazwisko)}@kangain.com".ToLower();

                    nowiTrenerzyUzytkownicy.Add(new Uzytkownik
                    {
                        Email = email,
                        Haslo = zahaszowaneHaslo,
                        DataRejestracji = faker.Date.Past(1),
                        RolaId = idRoliTrener,
                        Profil = new ProfilUzytkownika { Imie = imie, Nazwisko = nazwisko, Telefon = faker.Phone.PhoneNumber("#########") }
                    });
                }

                foreach (var chunk in nowiTrenerzyUzytkownicy.Chunk(100))
                {
                    context.Uzytkownicy.AddRange(chunk);
                    await context.SaveChangesAsync();
                }

                var wpisyPracownikow = nowiTrenerzyUzytkownicy.Select(t => new Pracownik
                {
                    UzytkownikId = t.Id,
                    Specjalizacja = t.Email == "trener.test@kangain.com" ? "Joga" : faker.PickRandom(dostepneSpecjalizacje),
                    DataZatrudnienia = faker.Date.Past(2)
                }).ToList();

                context.Pracownicy.AddRange(wpisyPracownikow);
                await context.SaveChangesAsync();
            }

            var janTrenerWpis = await context.Pracownicy
                .Include(p => p.Uzytkownik)
                .FirstOrDefaultAsync(p => p.Uzytkownik.Email == "trener.test@kangain.com");

            if (janTrenerWpis != null && janTrenerWpis.Specjalizacja != "Joga")
            {
                janTrenerWpis.Specjalizacja = "Joga";
                await context.SaveChangesAsync();
            }

            // --- 9. UZUPEŁNIANIE KLIENTÓW DO LICZBY 2000 (SZTYWNY PODZIAŁ NA TWOJE 6 PLANÓW) ---
            int obecniKlienciKonta = await context.Uzytkownicy.CountAsync(u => u.RolaId == idRoliUzytkownik);
            int iluKlientowDopisac = 2000 - obecniKlienciKonta;

            if (iluKlientowDopisac > 0)
            {
                int rozmiarPaczki = 500;
                for (int p = 0; p < iluKlientowDopisac; p += rozmiarPaczki)
                {
                    var paczkaUzytkownikow = new List<Uzytkownik>();
                    var paczkaSubskrypcji = new List<Subskrypcja>();
                    var paczkaPlatnosci = new List<Platnosc>();

                    int generujWTejPaczce = Math.Min(rozmiarPaczki, iluKlientowDopisac - p);

                    for (int i = 0; i < generujWTejPaczce; i++)
                    {
                        var imie = faker.Name.FirstName();
                        var nazwisko = faker.Name.LastName();
                        var email = faker.Internet.Email(imie, nazwisko).ToLower();

                        var u = new Uzytkownik
                        {
                            Email = email,
                            Haslo = zahaszowaneHaslo,
                            DataRejestracji = faker.Date.Past(2),
                            RolaId = idRoliUzytkownik,
                            Profil = new ProfilUzytkownika { Imie = imie, Nazwisko = nazwisko, Telefon = faker.Phone.PhoneNumber("#########") }
                        };
                        paczkaUzytkownikow.Add(u);

                        if (faker.Random.Bool(0.75f) && wszystkieTypyKarnetow.Any())
                        {
                            // Losuje dokładnie z puli Twoich kompletnych 6 planów z bazy danych
                            var typ = faker.PickRandom(wszystkieTypyKarnetow);
                            var dataStartu = faker.Date.Recent(30);

                            var sub = new Subskrypcja
                            {
                                Uzytkownik = u,
                                TypKarnetuId = typ.Id,
                                DataStartu = dataStartu,
                                DataKonca = dataStartu.AddDays(typ.CzasTrwaniaDni)
                            };
                            paczkaSubskrypcji.Add(sub);

                            paczkaPlatnosci.Add(new Platnosc
                            {
                                Subskrypcja = sub,
                                Kwota = typ.Cena,
                                DataPlatnosci = dataStartu,
                                Metoda = "Stripe"
                            });
                        }
                    }

                    context.Uzytkownicy.AddRange(paczkaUzytkownikow);
                    context.Subskrypcje.AddRange(paczkaSubskrypcji);
                    context.Platnosci.AddRange(paczkaPlatnosci);
                    await context.SaveChangesAsync();
                }
            }

            // --- 10. REKLAMOWY HARMONOGRAM GRAFIKU (10 MIASTO/DZIEŃ) ---
            context.Grafiki.RemoveRange(context.Grafiki);
            await context.SaveChangesAsync();

            var harmonogramGrafiku = new List<Grafik>();
            var wszyscyPracownicyEntities = await context.Pracownicy.ToListAsync();

            DateTime dataStartuMiesiaca = new DateTime(2026, 6, 8, 8, 0, 0);
            int[] cityCounters = new int[4];
            var zajeteSaleGlobalnie = new HashSet<string>();

            foreach (var trener in wszyscyPracownicyEntities)
            {
                var pasujaceZajecie = wszystkieZajeciaGrupowe.FirstOrDefault(z => z.Nazwa.Equals(trener.Specjalizacja, StringComparison.OrdinalIgnoreCase));
                if (pasujaceZajecie == null) continue;

                string oczekiwanaNazwaSali = trener.Specjalizacja.ToLower() switch
                {
                    "joga" => "Sala Joga",
                    "box" => "Sala Boksu",
                    "pilates" => "Sala Pilates",
                    "zumba" => "Sala Zajec Grupowych",
                    "crossfit" => "Sala Crossfit",
                    "silownia" => "Sala Silowa",
                    _ => "Sala Zajec Grupowych"
                };

                for (int dzien = 0; dzien < 4; dzien++)
                {
                    var dostepneGodzinyDnia = Enumerable.Range(8, 10).ToList();
                    var wybraneGodziny = faker.Random.Shuffle(dostepneGodzinyDnia).Take(5).ToList();

                    foreach (var godzina in wybraneGodziny)
                    {
                        var wybranaLokalizacja = wszystkieLokalizacjeZDatabase[cityCounters[dzien] % wszystkieLokalizacjeZDatabase.Count];

                        var dopasowanaSala = obecneSaleZDatabase.FirstOrDefault(s => s.LokalizacjaId == wybranaLokalizacja.Id && s.Nazwa.Equals(oczekiwanaNazwaSali, StringComparison.OrdinalIgnoreCase));
                        if (dopasowanaSala == null)
                        {
                            dopasowanaSala = obecneSaleZDatabase.FirstOrDefault(s => s.LokalizacjaId == wybranaLokalizacja.Id);
                        }

                        if (dopasowanaSala != null)
                        {
                            string kluczSali = $"{wybranaLokalizacja.Id}_{dopasowanaSala.Id}_{dzien}_{godzina}";
                            int proby = 0;
                            int finalnaGodzina = godzina;

                            while (zajeteSaleGlobalnie.Contains(kluczSali) && proby < 10)
                            {
                                var alternatywneGodziny = Enumerable.Range(8, 10).Where(h => !wybraneGodziny.Contains(h)).ToList();
                                if (!alternatywneGodziny.Any()) break;
                                finalnaGodzina = faker.PickRandom(alternatywneGodziny);
                                kluczSali = $"{wybranaLokalizacja.Id}_{dopasowanaSala.Id}_{dzien}_{finalnaGodzina}";
                                proby++;
                            }

                            DateTime startZajec = dataStartuMiesiaca.AddDays(dzien).AddHours(finalnaGodzina - 8);

                            harmonogramGrafiku.Add(new Grafik
                            {
                                PracownikId = trener.Id,
                                ZajeciaGrupoweId = pasujaceZajecie.Id,
                                SalaId = dopasowanaSala.Id,
                                DataStart = startZajec,
                                DataKoniec = startZajec.AddHours(1)
                            });

                            zajeteSaleGlobalnie.Add(kluczSali);
                            cityCounters[dzien]++;
                        }
                    }
                }
            }

            foreach (var chunk in harmonogramGrafiku.Chunk(1000))
            {
                context.Grafiki.AddRange(chunk);
                await context.SaveChangesAsync();
            }
        }

        private static string HaszujHaslo(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return string.Concat(bytes.Select(b => b.ToString("x2")));
            }
        }

        private static List<string> PobierzSzablonSprzetuZObrazka()
        {
            var lista = new List<string>();
            for (int waga = 5; waga <= 50; waga += 5) { lista.Add($"Hantle {waga}kg"); }
            lista.AddRange(new[] { "Ławka płaska", "Ławka skośna", "Ławka ujemna", "Ławka do brzuszków", "Ławka do wyciskania na barki", "Ławka do wyciskania na klatkę piersiową" });
            lista.AddRange(new[] { "Maszyna do wyciskania na klatkę piersiową", "Maszyna do przysiadów", "Maszyna do wiosłowania", "Maszyna do prostowania nóg", "Maszyna do zginania nóg", "Brama do podciągania", "Bieżnia", "Rower stacjonarny", "Orbitrek", "Stepper", "Sztanga olimpijska", "Sztanga standardowa", "Drążek do podciągania" });
            for (int w = 8; w <= 24; w += 4) { lista.Add($"Kettlebell {w}kg"); }
            for (int p = 5; p <= 25; p += 5) { lista.Add($"Piłka lekarska {p}kg"); }
            lista.AddRange(new[] { "Wyciąg górny", "Wyciąg dolny", "Wyciąg boczny", "Wyciąg do tricepsów", "Wyciąg do bicepsów" });
            return lista;
        }

        private static List<Cwiczenie> PrzygotujMaseCwiczen()
        {
            return new List<Cwiczenie>
            {
                new Cwiczenie { Nazwa = "Przysiad", Kategoria = "Silowe", Opis = "Podstawowe ćwiczenie na nogi, angażujące mięśnie czworogłowe i pośladkowe." },
                new Cwiczenie { Nazwa = "Martwy ciąg", Kategoria = "Silowe", Opis = "Ćwiczenie angażujące mięśnie pleców, pośladków, uda oraz korpusu." },
                new Cwiczenie { Nazwa = "Wyciskanie sztangi na ławce", Kategoria = "Silowe", Opis = "Ćwiczenie na klatkę piersiową, tricepsy i przednie aktony barków."},
                new Cwiczenie { Nazwa = "Podciąganie na drążku", Kategoria = "Silowe", Opis = "Ćwiczenie na plecy, bicepsy i przedramiona." },
                new Cwiczenie { Nazwa = "Pompki", Kategoria = "Silowe", Opis = "Ćwiczenie angażujące klatkę piersiową, tricepsy i przedni akton barku." },
                new Cwiczenie { Nazwa = "Bieg", Kategoria = "Cardio", Opis = "Ćwiczenie poprawiające wydolność sercowo-naczyniową i spalające kalorie." },
                new Cwiczenie { Nazwa = "Rower stacjonarny", Kategoria = "Cardio", Opis = "Ćwiczenie na poprawę kondycji i spalanie kalorii, angażujące nogi." },
                new Cwiczenie { Nazwa = "Skakanka", Kategoria = "Cardio", Opis = "Ćwiczenie poprawiające koordynację, kondycję i spalające kalorie." },
                new Cwiczenie { Nazwa = "Joga", Kategoria = "Rozciagajace", Opis = "Ćwiczenie poprawiające elastyczność, równowagę i redukujące stres." },
                new Cwiczenie { Nazwa = "Pilates", Kategoria = "Rozciagajace", Opis = "Ćwiczenie wzmacniające mięśnie głębokie, poprawiające postawę." },
                new Cwiczenie { Nazwa = "Stretching", Kategoria = "Rozciagajace", Opis = "Ćwiczenie poprawiające elastyczność mięśni i zakres ruchu w stawach." },
                new Cwiczenie { Nazwa = "Tai Chi", Kategoria = "Rozciagajace", Opis = "Ćwiczenie poprawiające równowagę, koordynację i redukujące stres." },
                new Cwiczenie { Nazwa = "Foam Rolling", Kategoria = "Rozciagajace", Opis = "Ćwiczenie pomagające w rozluźnieniu mięśni i poprawiające regenerację." },
                new Cwiczenie { Nazwa = "Trening obwodowy", Kategoria = "Mieszane", Opis = "Ćwiczenie łączące elementy siłowe i cardio, angażujące całe ciało." },
                new Cwiczenie { Nazwa = "Tabata", Kategoria = "Mieszane", Opis = "Intensywny trening interwałowy, który poprawia wydolność tlenową." },
                new Cwiczenie { Nazwa = "CrossFit", Kategoria = "Mieszane", Opis = "Trening łączący elementy siłowe, cardio i gimnastyczne." },
                new Cwiczenie { Nazwa = "HIIT (High-Intensity Interval Training)", Kategoria = "Mieszane", Opis = "Trening interwałowy o wysokiej intensywności, który przyspiesza metabolizm." },
                new Cwiczenie { Nazwa = "Circuit Training", Kategoria = "Mieszane", Opis = "Trening obwodowy, który łączy różne ćwiczenia w szybkim tempie." }
            };
        }

        private static List<(string Nazwa, string Adres, string Miasto, double Lat, double Lng)> PobierzSiatkeKlubowSwiat150()
        {
            return new List<(string Nazwa, string Adres, string Miasto, double Lat, double Lng)>
            {
                // POLSKA - 50 KLUBÓW
                ("KanGain Białystok", "ul. Wiejska 10", "Białystok", 53.1172, 23.1465),
                ("KanGain Warszawa", "ul. Marszałkowska 20", "Warszawa", 52.2185, 21.0215),
                ("KanGain Kraków", "ul. Floriańska 5", "Kraków", 50.0622, 19.9395),
                ("KanGain Gdańsk", "ul. Prosta 15", "Gdańsk", 54.3541, 18.6475),
                ("KanGain Wrocław", "ul. Świdnicka 30", "Wrocław", 51.1065, 17.0325),
                ("KanGain Poznań", "ul. Półwiejska 25", "Poznań", 52.4022, 16.9272),
                ("KanGain Łódź", "ul. Piotrkowska 100", "Łódź", 51.7645, 19.4585),
                ("KanGain Szczecin", "ul. Krzywoustego 50", "Szczecin", 53.4245, 14.5425),
                ("KanGain Lublin", "ul. Krakowskie Przedmieście 10", "Lublin", 51.2482, 22.5655),
                ("KanGain Katowice", "ul. 3 Maja 15", "Katowice", 50.2592, 19.0205),
                ("KanGain Bydgoszcz", "ul. Gdańska 40", "Bydgoszcz", 53.1282, 18.0055),
                ("KanGain Rzeszów", "ul. Popiełuszki 20", "Rzeszów", 50.0245, 22.0155),
                ("KanGain Olsztyn", "ul. Długa 10", "Olsztyn", 53.7742, 20.4855),
                ("KanGain Gdynia", "ul. Świętojańska 45", "Gdynia", 54.5189, 18.5305),
                ("KanGain Sopot", "ul. Bohaterów Monte Cassino 12", "Sopot", 54.4444, 18.5601),
                ("KanGain Toruń", "ul. Szeroka 22", "Toruń", 53.0138, 18.5984),
                ("KanGain Kielce", "ul. Sienkiewicza 35", "Kielce", 50.8703, 20.6278),
                ("KanGain Radom", "ul. Żeromskiego 18", "Radom", 51.4027, 21.1563),
                ("KanGain Częstochowa", "Al. NMP 24", "Częstochowa", 50.8118, 19.1204),
                ("KanGain Gliwice", "ul. Zwycięstwa 15", "Gliwice", 50.2945, 18.6655),
                ("KanGain Zabrze", "ul. Wolności 120", "Zabrze", 50.3081, 18.7819),
                ("KanGain Sosnowiec", "ul. Modrzejowska 8", "Sosnowiec", 50.2866, 19.1325),
                ("KanGain Zielona Góra", "ul. Kupiecka 30", "Zielona Góra", 51.9355, 15.5064),
                ("KanGain Opole", "ul. Krakowska 14", "Opole", 50.6664, 17.9236),
                ("KanGain Gorzów Wielkopolski", "ul. Chrobrego 5", "Gorzów Wlkp.", 52.7368, 15.2288),
                ("KanGain Elbląg", "ul. Hetmańska 19", "Elbląg", 54.1561, 19.4044),
                ("KanGain Wałbrzych", "ul. Główna 44", "Wałbrzych", 50.7833, 16.2833),
                ("KanGain Włocławek", "ul. Kościuszki 10", "Włocławek", 52.6484, 19.0678),
                ("KanGain Tarnów", "ul. Wałowa 5", "Tarnów", 50.0139, 20.9889),
                ("KanGain Chorzów", "ul. Wolności 50", "Chorzów", 50.2976, 18.9511),
                ("KanGain Koszalin", "ul. Zwycięstwa 80", "Koszalin", 54.1944, 16.1722),
                ("KanGain Kalisz", "Główny Rynek 12", "Kalisz", 51.7611, 18.0911),
                ("KanGain Legnica", "ul. NMP 5", "Legnica", 51.2070, 16.1611),
                ("KanGain Grudziądz", "ul. Włodka 11", "Grudziądz", 53.4842, 18.7536),
                ("KanGain Słupsk", "ul. Wojska Polskiego 4", "Słupsk", 54.4641, 17.0284),
                ("KanGain Jaworzno", "ul. Grunwaldzka 55", "Jaworzno", 50.2051, 19.2742),
                ("KanGain Jastrzębie-Zdrój", "ul. Północna 10", "Jastrzębie-Zdrój", 49.9531, 18.5914),
                ("KanGain Nowy Sącz", "Rynek 3", "Nowy Sącz", 49.6253, 20.6924),
                ("KanGain Jelenia Góra", "ul. 1 Maja 15", "Jelenia Góra", 50.9044, 15.7394),
                ("KanGain Siedlce", "ul. Piłsudskiego 40", "Siedlce", 52.1678, 22.2903),
                ("KanGain Konin", "ul. Kolejowa 5", "Konin", 52.2234, 18.2514),
                ("KanGain Piotrków Trybunalski", "ul. Słowackiego 12", "Piotrków Tryb.", 51.4053, 19.6833),
                ("KanGain Inowrocław", "ul. Królowej Jadwigi 4", "Inowrocław", 52.7955, 18.2614),
                ("KanGain Mysłowice", "ul. Grunwaldzka 2", "Mysłowice", 50.2414, 19.1347),
                ("KanGain Piła", "ul. Śródmiejska 15", "Piła", 53.1514, 16.7384),
                ("KanGain Lubin", "ul. Niepodległości 20", "Lubin", 51.3964, 16.2058),
                ("KanGain Ostrów Wielkopolski", "Rynek 22", "Ostrów Wlkp.", 51.6514, 17.8136),
                ("KanGain Suwałki", "ul. Chłodna 3", "Suwałki", 54.1114, 22.9306),
                ("KanGain Gniezno", "ul. Chrobrego 14", "Gniezno", 52.5347, 17.6003),
                ("KanGain Stargard", "ul. Wyszyńskiego 5", "Stargard", 53.3364, 15.0458),

                // EUROPA - 50 KLUBÓW
                ("KanGain Berlin", "Alexanderplatz 4", "Berlin", 52.5200, 13.4050),
                ("KanGain Hamburg", "Reeperbahn 12", "Hamburg", 53.5511, 9.9937),
                ("KanGain Munich", "Marienplatz 2", "München", 48.1351, 11.5820),
                ("KanGain Paris", "Champs-Élysées 75", "Paris", 48.8566, 2.3522),
                ("KanGain London", "Oxford St 200", "London", 51.5074, -0.1278),
                ("KanGain Rome", "Via del Corso 12", "Rome", 41.9028, 12.4964),
                ("KanGain Madrid", "Gran Via 30", "Madrid", 40.4167, -3.7037),
                ("KanGain Bratislava", "Obchodná 7", "Bratislava", 48.1486, 17.1077),
                ("KanGain Zagreb", "Ilica 40", "Zagreb", 45.8150, 15.9819),
                ("KanGain Split", "Riva 3", "Split", 43.5081, 16.4392),
                ("KanGain Vienna", "Graben 15", "Wien", 48.2082, 16.3738),
                ("KanGain Prague", "Na Příkopě 10", "Praha", 50.0878, 14.4205),
                ("KanGain Budapest", "Váci utca 25", "Budapest", 47.4979, 19.0402),
                ("KanGain Amsterdam", "Damrak 45", "Amsterdam", 52.3702, 4.8952),
                ("KanGain Brussels", "Rue Neuve 50", "Bruxelles", 50.8503, 4.3517),
                ("KanGain Luxembourg", "Grand-Rue 12", "Luxembourg", 49.6116, 6.1319),
                ("KanGain Copenhagen", "Strøget 15", "København", 55.6761, 12.5683),
                ("KanGain Stockholm", "Drottninggatan 30", "Stockholm", 59.3293, 18.0686),
                ("KanGain Oslo", "Karl Johans gate 10", "Oslo", 59.9139, 10.7522),
                ("KanGain Helsinki", "Aleksanterinkatu 20", "Helsinki", 60.1699, 24.9384),
                ("KanGain Lisbon", "Augusta St 45", "Lisboa", 38.7223, -9.1393),
                ("KanGain Dublin", "Grafton St 5", "Dublin", 53.3498, -6.2603),
                ("KanGain Athens", "Ermou St 12", "Athinai", 37.9838, 23.7275),
                ("KanGain Milan", "Via Torino 14", "Milano", 45.4642, 9.1900),
                ("KanGain Barcelona", "La Rambla 80", "Barcelona", 41.3851, 2.1734),
                ("KanGain Lyon", "Rue de la République 12", "Lyon", 45.7500, 4.8500),
                ("KanGain Marseille", "La Canebière 5", "Marseille", 43.2965, 5.3698),
                ("KanGain Frankfurt", "Zeil 85", "Frankfurt", 50.1109, 8.6821),
                ("KanGain Cologne", "Hohe Straße 45", "Köln", 50.9375, 6.9603),
                ("KanGain Zurich", "Bahnhofstrasse 40", "Zürich", 47.3769, 8.5417),
                ("KanGain Geneva", "Rue du Rhône 15", "Genève", 46.2044, 6.1432),
                ("KanGain Graz", "Herrengasse 5", "Graz", 47.0707, 15.4395),
                ("KanGain Linz", "Landstraße 20", "Linz", 48.3064, 14.2858),
                ("KanGain Salzburg", "Getreidegasse 3", "Salzburg", 47.8095, 13.0550),
                ("KanGain Brno", "Náměstí Svobody 12", "Brno", 49.1951, 16.6068),
                ("KanGain Ostrava", "Stodolní 5", "Ostrava", 49.8209, 18.2625),
                ("KanGain Debrecen", "Piac utca 10", "Debrecen", 47.5316, 21.6273),
                ("KanGain Szeged", "Kárász utca 4", "Szeged", 46.2530, 20.1414),
                ("KanGain Kosice", "Hlavná 22", "Košice", 48.7266, 21.2571),
                ("KanGain Presov", "Hlavná 45", "Prešov", 48.9984, 21.2393),
                ("KanGain Zilina", "Národná 12", "Żilina", 49.2232, 18.7394),
                ("KanGain Nitra", "Štefánikova 8", "Nitra", 48.3061, 18.0764),
                ("KanGain Rijeka", "Korzo 11", "Rijeka", 45.3271, 14.4422),
                ("KanGain Osijek", "Kapucinska 5", "Osijek", 45.5550, 18.6931),
                ("KanGain Zadar", "Kalelarga 8", "Zadar", 44.1194, 15.2281),
                ("KanGain Birmingham", "New St 44", "Birmingham", 52.4862, -1.8904),
                ("KanGain Manchester", "Deansgate 100", "Manchester", 53.4808, -2.2426),
                ("KanGain Glasgow", "Buchanan St 85", "Glasgow", 55.8642, -4.2518),
                ("KanGain Liverpool", "Bold St 12", "Liverpool", 53.4084, -2.9916),
                ("KanGain Edinburgh", "Princes St 50", "Edinburgh", 55.9533, -3.1883),

                // ŚWIAT - 50 KLUBÓW
                ("KanGain New York", "5th Ave 350", "New York", 40.7484, -73.9857),
                ("KanGain Los Angeles", "Sunset Blvd 6200", "Los Angeles", 34.0522, -118.2437),
                ("KanGain Chicago", "Michigan Ave 400", "Chicago", 41.8781, -87.6298),
                ("KanGain Houston", "Main St 1000", "Houston", 29.7604, -95.3698),
                ("KanGain Miami", "Ocean Drive 500", "Miami", 25.7617, -80.1918),
                ("KanGain San Francisco", "Market St 700", "San Francisco", 37.7749, -122.4194),
                ("KanGain Boston", "Boylston St 500", "Boston", 42.3601, -71.0589),
                ("KanGain Seattle", "Pine St 400", "Seattle", 47.6062, -122.3321),
                ("KanGain Washington", "Pennsylvania Ave 1600", "Washington", 38.9072, -77.0369),
                ("KanGain Las Vegas", "Las Vegas Blvd 3500", "Las Vegas", 36.1699, -115.1398),
                ("KanGain Toronto", "Yonge St 250", "Toronto", 43.6532, -79.3832),
                ("KanGain Vancouver", "Robson St 800", "Vancouver", 49.2827, -123.1207),
                ("KanGain Montreal", "Sainte-Catherine 400", "Montreal", 45.5017, -73.5673),
                ("KanGain Mexico City", "Reforma 200", "Mexico City", 19.4326, -99.1332),
                ("KanGain Cancun", "Kukulcan Blvd 5", "Cancun", 21.1619, -86.8515),
                ("KanGain Sao Paulo", "Paulista Ave 1000", "Sao Paulo", -23.5505, -46.6333),
                ("KanGain Rio de Janeiro", "Copacabana 500", "Rio de Janeiro", -22.9068, -43.1729),
                ("KanGain Buenos Aires", "Florida St 300", "Buenos Aires", -34.6037, -58.3816),
                ("KanGain Santiago", "Providencia 1200", "Santiago", -33.4489, -70.6693),
                ("KanGain Bogota", "Carrera 7 50", "Bogota", 4.7110, -74.0721),
                ("KanGain Lima", "Larco Ave 400", "Lima", -12.0464, -77.0428),
                ("KanGain Tokyo", "Shibuya 2-21", "Tokyo", 35.6895, 13.6917),
                ("KanGain Osaka", "Umeda 1-1", "Osaka", 34.6937, 135.5023),
                ("KanGain Kyoto", "Shijo St 50", "Kyoto", 35.0116, 135.7681),
                ("KanGain Seoul", "Gangnam-daero 400", "Seoul", 37.5665, 126.9780),
                ("KanGain Busan", "Haeundae 50", "Busan", 35.1796, 129.0756),
                ("KanGain Beijing", "Wangfujing 10", "Beijing", 39.9042, 116.4074),
                ("KanGain Shanghai", "Nanjing Rd 500", "Shanghai", 31.2304, 121.4737),
                ("KanGain Hong Kong", "Nathan Rd 300", "Hong Kong", 22.3193, 114.1694),
                ("KanGain Singapore", "Orchard Rd 250", "Singapore", 1.3521, 103.8198),
                ("KanGain Bangkok", "Sukhumvit Rd 20", "Bangkok", 13.7563, 100.5018),
                ("KanGain Kuala Lumpur", "Bukit Bintang 150", "Kuala Lumpur", 3.1390, 101.6869),
                ("KanGain Jakarta", "Sudirman 40", "Jakarta", -6.2088, 106.8456),
                ("KanGain Manila", "Ayala Ave 50", "Manila", 14.5995, 120.9842),
                ("KanGain Sydney", "George St 400", "Sydney", -33.8688, 151.2093),
                ("KanGain Melbourne", "Flinders St 200", "Melbourne", -37.8136, 144.9631),
                ("KanGain Brisbane", "Queen St 150", "Brisbane", -27.4698, 153.0251),
                ("KanGain Auckland", "Queen St 50", "Auckland", -36.8485, 174.7633),
                ("KanGain Wellington", "Lambton Quay 100", "Wellington", -41.2865, 174.7762),
                ("KanGain Dubai", "Sheikh Zayed Rd 50", "Dubai", 25.2048, 55.2708),
                ("KanGain Abu Dhabi", "Corniche Rd 12", "Abu Dhabi", 24.4539, 54.3773),
                ("KanGain Tel Aviv", "Rothschild Blvd 40", "Tel Aviv", 32.0853, 34.7818),
                ("KanGain Cairo", "Tahrir Sq 5", "Cairo", 30.0444, 31.2357),
                ("KanGain Cape Town", "Long St 100", "Cape Town", -33.9249, 18.4241),
                ("KanGain Johannesburg", "Fox St 200", "Johannesburg", -26.2041, 28.0473),
                ("KanGain Casablanca", "Anfa Blvd 50", "Casablanca", 33.5731, -7.5898),
                ("KanGain Nairobi", "Moi Ave 20", "Nairobi", -1.2921, 36.8219),
                ("KanGain Mumbai", "Colaba Causeway 15", "Mumbai", 18.9220, 72.8346),
                ("KanGain New Delhi", "Connaught Place 5", "New Delhi", 28.6139, 77.2090),
                ("KanGain Bangalore", "MG Road 80", "Bangalore", 12.9716, 77.5946)
            };
        }
    }
}