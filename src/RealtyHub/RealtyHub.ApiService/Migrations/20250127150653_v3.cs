using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RealtyHub.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ContractContent",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "ContractContent",
                columns: new[] { "Id", "Extension", "IsActive", "Name", "Type", "UserId" },
                values: new object[,]
                {
                    { "2824aec3-3219-4d81-a97a-c3b80ca72844", ".pdf", true, "Modelo Padrão", 0, "" },
                    { "2f4c556d-6850-4b3d-afe9-d7c2bd282718", ".pdf", true, "Modelo de Contrato - PFPJ", 3, "" },
                    { "a2c16556-5098-4496-ae7a-1f9b6d0e8fcf", ".pdf", true, "Modelo de Contrato - PJPJ", 1, "" },
                    { "f7581a63-f4f0-4881-b6ed-6a4100b4182e", ".pdf", true, "Modelo de Contrato - PFPF", 2, "" },
                    { "fd7ed50d-8f42-4288-8877-3cb8095370e7", ".pdf", true, "Modelo de Contrato - PJPF", 4, "" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "2824aec3-3219-4d81-a97a-c3b80ca72844");

            migrationBuilder.DeleteData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "2f4c556d-6850-4b3d-afe9-d7c2bd282718");

            migrationBuilder.DeleteData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "a2c16556-5098-4496-ae7a-1f9b6d0e8fcf");

            migrationBuilder.DeleteData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "f7581a63-f4f0-4881-b6ed-6a4100b4182e");

            migrationBuilder.DeleteData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "fd7ed50d-8f42-4288-8877-3cb8095370e7");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ContractContent");
        }
    }
}
