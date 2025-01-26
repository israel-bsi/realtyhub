using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtyHub.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Viewing_Customer_CustomerId",
                table: "Viewing");

            migrationBuilder.DropForeignKey(
                name: "FK_Viewing_Property_PropertyId",
                table: "Viewing");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Viewing",
                newName: "PropertyId1");

            migrationBuilder.RenameIndex(
                name: "IX_Viewing_CustomerId",
                table: "Viewing",
                newName: "IX_Viewing_PropertyId1");

            migrationBuilder.AddColumn<long>(
                name: "BuyerId",
                table: "Viewing",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "RegistryNumber",
                table: "Property",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegistryRecord",
                table: "Property",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Viewing_Customer_PropertyId",
                table: "Viewing",
                column: "PropertyId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Viewing_Property_PropertyId1",
                table: "Viewing",
                column: "PropertyId1",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Viewing_Customer_PropertyId",
                table: "Viewing");

            migrationBuilder.DropForeignKey(
                name: "FK_Viewing_Property_PropertyId1",
                table: "Viewing");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Viewing");

            migrationBuilder.DropColumn(
                name: "RegistryNumber",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "RegistryRecord",
                table: "Property");

            migrationBuilder.RenameColumn(
                name: "PropertyId1",
                table: "Viewing",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Viewing_PropertyId1",
                table: "Viewing",
                newName: "IX_Viewing_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Viewing_Customer_CustomerId",
                table: "Viewing",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Viewing_Property_PropertyId",
                table: "Viewing",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
