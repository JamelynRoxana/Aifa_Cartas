using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartas_aifa.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFechaLimiteCodigo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCodigoAcceso",
                table: "Estudiantes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLimiteRegistro",
                table: "CodigosAcceso",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_IdCodigoAcceso",
                table: "Estudiantes",
                column: "IdCodigoAcceso");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_CodigosAcceso_IdCodigoAcceso",
                table: "Estudiantes",
                column: "IdCodigoAcceso",
                principalTable: "CodigosAcceso",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_CodigosAcceso_IdCodigoAcceso",
                table: "Estudiantes");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_IdCodigoAcceso",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "IdCodigoAcceso",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "FechaLimiteRegistro",
                table: "CodigosAcceso");
        }
    }
}
