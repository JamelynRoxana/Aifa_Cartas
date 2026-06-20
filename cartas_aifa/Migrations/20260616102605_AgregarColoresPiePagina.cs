using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartas_aifa.Migrations
{
    /// <inheritdoc />
    public partial class AgregarColoresPiePagina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorFondo",
                table: "PiesDePagina",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorLetra",
                table: "PiesDePagina",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorFondo",
                table: "PiesDePagina");

            migrationBuilder.DropColumn(
                name: "ColorLetra",
                table: "PiesDePagina");
        }
    }
}
