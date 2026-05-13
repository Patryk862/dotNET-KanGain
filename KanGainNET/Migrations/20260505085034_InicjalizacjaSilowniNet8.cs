using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanGainNET.Migrations
{
    /// <inheritdoc />
    public partial class InicjalizacjaSilowniNet8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cwiczenia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cwiczenia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lokalizacje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Miasto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lokalizacje", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rabaty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcentZnizki = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rabaty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypyKarnetow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cena = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CzasTrwaniaDni = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypyKarnetow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZajeciaGrupowe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZajeciaGrupowe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pojemnosc = table.Column<int>(type: "int", nullable: false),
                    LokalizacjaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sale_Lokalizacje_LokalizacjaId",
                        column: x => x.LokalizacjaId,
                        principalTable: "Lokalizacje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sprzet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StanTechniczny = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LokalizacjaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprzet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sprzet_Lokalizacje_LokalizacjaId",
                        column: x => x.LokalizacjaId,
                        principalTable: "Lokalizacje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Uzytkownicy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Haslo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataRejestracji = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RolaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzytkownicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Uzytkownicy_Role_RolaId",
                        column: x => x.RolaId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Obecnosci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataWejscia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataWyjscia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obecnosci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obecnosci_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PomiaryCiala",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataPomiaru = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Waga = table.Column<double>(type: "float", nullable: false),
                    TkankaTluszczowa = table.Column<double>(type: "float", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PomiaryCiala", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PomiaryCiala_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pracownicy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Specjalizacja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataZatrudnienia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pracownicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pracownicy_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileUzytkownikow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileUzytkownikow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileUzytkownikow_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subskrypcje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataStartu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataKonca = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    TypKarnetuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subskrypcje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subskrypcje_TypyKarnetow_TypKarnetuId",
                        column: x => x.TypKarnetuId,
                        principalTable: "TypyKarnetow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subskrypcje_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Grafiki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataGodzina = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZajeciaGrupoweId = table.Column<int>(type: "int", nullable: false),
                    SalaId = table.Column<int>(type: "int", nullable: false),
                    PracownikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grafiki", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grafiki_Pracownicy_PracownikId",
                        column: x => x.PracownikId,
                        principalTable: "Pracownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grafiki_Sale_SalaId",
                        column: x => x.SalaId,
                        principalTable: "Sale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grafiki_ZajeciaGrupowe_ZajeciaGrupoweId",
                        column: x => x.ZajeciaGrupoweId,
                        principalTable: "ZajeciaGrupowe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanyTreningowe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataStworzenia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    PracownikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanyTreningowe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanyTreningowe_Pracownicy_PracownikId",
                        column: x => x.PracownikId,
                        principalTable: "Pracownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanyTreningowe_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Platnosci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kwota = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataPlatnosci = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metoda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubskrypcjaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platnosci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Platnosci_Subskrypcje_SubskrypcjaId",
                        column: x => x.SubskrypcjaId,
                        principalTable: "Subskrypcje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rezerwacje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    GrafikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezerwacje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezerwacje_Grafiki_GrafikId",
                        column: x => x.GrafikId,
                        principalTable: "Grafiki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rezerwacje_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grafiki_PracownikId",
                table: "Grafiki",
                column: "PracownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Grafiki_SalaId",
                table: "Grafiki",
                column: "SalaId");

            migrationBuilder.CreateIndex(
                name: "IX_Grafiki_ZajeciaGrupoweId",
                table: "Grafiki",
                column: "ZajeciaGrupoweId");

            migrationBuilder.CreateIndex(
                name: "IX_Obecnosci_UzytkownikId",
                table: "Obecnosci",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanyTreningowe_PracownikId",
                table: "PlanyTreningowe",
                column: "PracownikId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanyTreningowe_UzytkownikId",
                table: "PlanyTreningowe",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Platnosci_SubskrypcjaId",
                table: "Platnosci",
                column: "SubskrypcjaId");

            migrationBuilder.CreateIndex(
                name: "IX_PomiaryCiala_UzytkownikId",
                table: "PomiaryCiala",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Pracownicy_UzytkownikId",
                table: "Pracownicy",
                column: "UzytkownikId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileUzytkownikow_UzytkownikId",
                table: "ProfileUzytkownikow",
                column: "UzytkownikId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacje_GrafikId",
                table: "Rezerwacje",
                column: "GrafikId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacje_UzytkownikId",
                table: "Rezerwacje",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_LokalizacjaId",
                table: "Sale",
                column: "LokalizacjaId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprzet_LokalizacjaId",
                table: "Sprzet",
                column: "LokalizacjaId");

            migrationBuilder.CreateIndex(
                name: "IX_Subskrypcje_TypKarnetuId",
                table: "Subskrypcje",
                column: "TypKarnetuId");

            migrationBuilder.CreateIndex(
                name: "IX_Subskrypcje_UzytkownikId",
                table: "Subskrypcje",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Uzytkownicy_RolaId",
                table: "Uzytkownicy",
                column: "RolaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cwiczenia");

            migrationBuilder.DropTable(
                name: "Obecnosci");

            migrationBuilder.DropTable(
                name: "PlanyTreningowe");

            migrationBuilder.DropTable(
                name: "Platnosci");

            migrationBuilder.DropTable(
                name: "PomiaryCiala");

            migrationBuilder.DropTable(
                name: "ProfileUzytkownikow");

            migrationBuilder.DropTable(
                name: "Rabaty");

            migrationBuilder.DropTable(
                name: "Rezerwacje");

            migrationBuilder.DropTable(
                name: "Sprzet");

            migrationBuilder.DropTable(
                name: "Subskrypcje");

            migrationBuilder.DropTable(
                name: "Grafiki");

            migrationBuilder.DropTable(
                name: "TypyKarnetow");

            migrationBuilder.DropTable(
                name: "Pracownicy");

            migrationBuilder.DropTable(
                name: "Sale");

            migrationBuilder.DropTable(
                name: "ZajeciaGrupowe");

            migrationBuilder.DropTable(
                name: "Uzytkownicy");

            migrationBuilder.DropTable(
                name: "Lokalizacje");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
