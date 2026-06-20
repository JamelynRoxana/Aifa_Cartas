using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartas_aifa.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaLeyendas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leyendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alineacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoordX = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CoordY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TamanoFuente = table.Column<int>(type: "int", nullable: false),
                    EstiloFuente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MostrarEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leyendas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leyendas");
        }
    }
}
