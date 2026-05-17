using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanGainNET.Migrations
{
    /// <inheritdoc />
    public partial class TabeleSprzetu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sprzet_Lokalizacje_LokalizacjaId",
                table: "Sprzet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sprzet",
                table: "Sprzet");

            migrationBuilder.RenameTable(
                name: "Sprzet",
                newName: "Sprzety");

            migrationBuilder.RenameIndex(
                name: "IX_Sprzet_LokalizacjaId",
                table: "Sprzety",
                newName: "IX_Sprzety_LokalizacjaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sprzety",
                table: "Sprzety",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sprzety_Lokalizacje_LokalizacjaId",
                table: "Sprzety",
                column: "LokalizacjaId",
                principalTable: "Lokalizacje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sprzety_Lokalizacje_LokalizacjaId",
                table: "Sprzety");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sprzety",
                table: "Sprzety");

            migrationBuilder.RenameTable(
                name: "Sprzety",
                newName: "Sprzet");

            migrationBuilder.RenameIndex(
                name: "IX_Sprzety_LokalizacjaId",
                table: "Sprzet",
                newName: "IX_Sprzet_LokalizacjaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sprzet",
                table: "Sprzet",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sprzet_Lokalizacje_LokalizacjaId",
                table: "Sprzet",
                column: "LokalizacjaId",
                principalTable: "Lokalizacje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
