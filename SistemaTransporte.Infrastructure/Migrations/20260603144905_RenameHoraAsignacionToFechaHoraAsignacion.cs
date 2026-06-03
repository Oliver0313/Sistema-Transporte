using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTransporte.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameHoraAsignacionToFechaHoraAsignacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HoraAsignacion",
                table: "Asignaciones",
                newName: "FechaHoraAsignacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaHoraAsignacion",
                table: "Asignaciones",
                newName: "HoraAsignacion");
        }
    }
}
