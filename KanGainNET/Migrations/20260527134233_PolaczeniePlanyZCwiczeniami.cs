using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanGainNET.Migrations
{
    /// <inheritdoc />
    public partial class PolaczeniePlanyZCwiczeniami : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CwiczeniePlanTreningowy",
                columns: table => new
                {
                    CwiczeniaId = table.Column<int>(type: "int", nullable: false),
                    PlanyTreningoweId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CwiczeniePlanTreningowy", x => new { x.CwiczeniaId, x.PlanyTreningoweId });
                    table.ForeignKey(
                        name: "FK_CwiczeniePlanTreningowy_Cwiczenia_CwiczeniaId",
                        column: x => x.CwiczeniaId,
                        principalTable: "Cwiczenia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CwiczeniePlanTreningowy_PlanyTreningowe_PlanyTreningoweId",
                        column: x => x.PlanyTreningoweId,
                        principalTable: "PlanyTreningowe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CwiczeniePlanTreningowy_PlanyTreningoweId",
                table: "CwiczeniePlanTreningowy",
                column: "PlanyTreningoweId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CwiczeniePlanTreningowy");
        }
    }
}
