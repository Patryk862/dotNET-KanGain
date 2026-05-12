using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanGainNET.Migrations
{
    /// <inheritdoc />
    public partial class AktualizacjaGrafiku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataGodzina",
                table: "Grafiki",
                newName: "DataStart");

            migrationBuilder.AddColumn<decimal>(
                 name: "Dlugosc",
                 table: "Lokalizacje",
                 type: "decimal(18,2)",
                 nullable: true);

            migrationBuilder.AddColumn<decimal>(
                 name: "Szerokosc",
                 table: "Lokalizacje",
                 type: "decimal(18,2)",
                 nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ZajeciaGrupoweId",
                table: "Grafiki",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PracownikId",
                table: "Grafiki",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataKoniec",
                table: "Grafiki",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dlugosc",
                table: "Lokalizacje");

            migrationBuilder.DropColumn(
                name: "Szerokosc",
                table: "Lokalizacje");

            migrationBuilder.DropColumn(
                name: "DataKoniec",
                table: "Grafiki");

            migrationBuilder.RenameColumn(
                name: "DataStart",
                table: "Grafiki",
                newName: "DataGodzina");

            migrationBuilder.AlterColumn<int>(
                name: "ZajeciaGrupoweId",
                table: "Grafiki",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PracownikId",
                table: "Grafiki",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
