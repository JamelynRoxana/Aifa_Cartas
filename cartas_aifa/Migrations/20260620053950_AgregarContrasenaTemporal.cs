using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartas_aifa.Migrations
{
    /// <inheritdoc />
    public partial class AgregarContrasenaTemporal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ContrasenaTempUsada",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ContrasenaTemporal",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContrasenaTemporalExpira",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContrasenaTempUsada",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ContrasenaTemporal",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ContrasenaTemporalExpira",
                table: "Usuarios");
        }
    }
}
