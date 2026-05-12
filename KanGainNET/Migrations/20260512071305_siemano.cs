using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanGainNET.Migrations
{
    /// <inheritdoc />
    public partial class siemano : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Szerokosc",
                table: "Lokalizacje",
                type: "decimal(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Dlugosc",
                table: "Lokalizacje",
                type: "decimal(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Szerokosc",
                table: "Lokalizacje",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Dlugosc",
                table: "Lokalizacje",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,8)",
                oldNullable: true);
        }
    }
}
