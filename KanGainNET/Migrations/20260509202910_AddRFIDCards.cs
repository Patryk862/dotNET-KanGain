using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanGainNET.Migrations
{
    /// <inheritdoc />
    public partial class AddRFIDCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataWyjscia",
                table: "Obecnosci");

            migrationBuilder.RenameColumn(
                name: "DataWejscia",
                table: "Obecnosci",
                newName: "Data");

            migrationBuilder.AlterColumn<int>(
                name: "UzytkownikId",
                table: "Obecnosci",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "KartaRFIDId",
                table: "Obecnosci",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Typ",
                table: "Obecnosci",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "KartyRFID",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktywna = table.Column<bool>(type: "bit", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KartyRFID", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KartyRFID_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Obecnosci_KartaRFIDId",
                table: "Obecnosci",
                column: "KartaRFIDId");

            migrationBuilder.CreateIndex(
                name: "IX_KartyRFID_UzytkownikId",
                table: "KartyRFID",
                column: "UzytkownikId");

            migrationBuilder.AddForeignKey(
                name: "FK_Obecnosci_KartyRFID_KartaRFIDId",
                table: "Obecnosci",
                column: "KartaRFIDId",
                principalTable: "KartyRFID",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obecnosci_KartyRFID_KartaRFIDId",
                table: "Obecnosci");

            migrationBuilder.DropTable(
                name: "KartyRFID");

            migrationBuilder.DropIndex(
                name: "IX_Obecnosci_KartaRFIDId",
                table: "Obecnosci");

            migrationBuilder.DropColumn(
                name: "KartaRFIDId",
                table: "Obecnosci");

            migrationBuilder.DropColumn(
                name: "Typ",
                table: "Obecnosci");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Obecnosci",
                newName: "DataWejscia");

            migrationBuilder.AlterColumn<int>(
                name: "UzytkownikId",
                table: "Obecnosci",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataWyjscia",
                table: "Obecnosci",
                type: "datetime2",
                nullable: true);
        }
    }
}
