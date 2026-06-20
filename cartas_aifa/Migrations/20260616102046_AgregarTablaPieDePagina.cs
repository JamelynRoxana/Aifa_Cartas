using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartas_aifa.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaPieDePagina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PiesDePagina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TieneLinea = table.Column<bool>(type: "bit", nullable: false),
                    ColorLinea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrosorLinea = table.Column<float>(type: "real", nullable: false),
                    Alineacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TamanoFuente = table.Column<int>(type: "int", nullable: false),
                    MostrarEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PiesDePagina", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PiesDePagina");
        }
    }
}
