using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartas_aifa.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposEstudiante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NombreCompleto",
                table: "Estudiantes",
                newName: "Nombre");

            migrationBuilder.AddColumn<string>(
                name: "ApellidoMaterno",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApellidoPaterno",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Estudiantes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdDetalleEtapa",
                table: "Estudiantes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdDireccion",
                table: "Estudiantes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdSubdireccion",
                table: "Estudiantes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_IdDetalleEtapa",
                table: "Estudiantes",
                column: "IdDetalleEtapa");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_IdDireccion",
                table: "Estudiantes",
                column: "IdDireccion");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_IdSubdireccion",
                table: "Estudiantes",
                column: "IdSubdireccion");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_DetallesEtapas_IdDetalleEtapa",
                table: "Estudiantes",
                column: "IdDetalleEtapa",
                principalTable: "DetallesEtapas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_DireccionesAifa_IdDireccion",
                table: "Estudiantes",
                column: "IdDireccion",
                principalTable: "DireccionesAifa",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_SubdireccionesAifa_IdSubdireccion",
                table: "Estudiantes",
                column: "IdSubdireccion",
                principalTable: "SubdireccionesAifa",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_DetallesEtapas_IdDetalleEtapa",
                table: "Estudiantes");

            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_DireccionesAifa_IdDireccion",
                table: "Estudiantes");

            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_SubdireccionesAifa_IdSubdireccion",
                table: "Estudiantes");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_IdDetalleEtapa",
                table: "Estudiantes");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_IdDireccion",
                table: "Estudiantes");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_IdSubdireccion",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "ApellidoMaterno",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "ApellidoPaterno",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "IdDetalleEtapa",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "IdDireccion",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "IdSubdireccion",
                table: "Estudiantes");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Estudiantes",
                newName: "NombreCompleto");
        }
    }
}
