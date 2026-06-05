using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTransporte.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenUrlToVehiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgaenUrl",
                table: "Vehiculos",
                newName: "ImagenUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagenUrl",
                table: "Vehiculos",
                newName: "ImgaenUrl");
        }
    }
}
