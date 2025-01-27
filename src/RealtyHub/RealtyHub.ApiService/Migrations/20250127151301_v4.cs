using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtyHub.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowInPage",
                table: "ContractContent",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "2824aec3-3219-4d81-a97a-c3b80ca72844",
                column: "ShowInPage",
                value: true);

            migrationBuilder.UpdateData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "2f4c556d-6850-4b3d-afe9-d7c2bd282718",
                column: "ShowInPage",
                value: false);

            migrationBuilder.UpdateData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "a2c16556-5098-4496-ae7a-1f9b6d0e8fcf",
                column: "ShowInPage",
                value: false);

            migrationBuilder.UpdateData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "f7581a63-f4f0-4881-b6ed-6a4100b4182e",
                column: "ShowInPage",
                value: false);

            migrationBuilder.UpdateData(
                table: "ContractContent",
                keyColumn: "Id",
                keyValue: "fd7ed50d-8f42-4288-8877-3cb8095370e7",
                column: "ShowInPage",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowInPage",
                table: "ContractContent");
        }
    }
}
