using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartas_aifa.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIdUsuarioEstudiante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUsuario",
                table: "Estudiantes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_IdUsuario",
                table: "Estudiantes",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_Usuarios_IdUsuario",
                table: "Estudiantes",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_Usuarios_IdUsuario",
                table: "Estudiantes");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_IdUsuario",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Estudiantes");
        }
    }
}
