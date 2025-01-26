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
            migrationBuilder.DropForeignKey(
                name: "FK_Viewing_Customer_PropertyId",
                table: "Viewing");

            migrationBuilder.DropForeignKey(
                name: "FK_Viewing_Property_PropertyId1",
                table: "Viewing");

            migrationBuilder.DropIndex(
                name: "IX_Viewing_PropertyId1",
                table: "Viewing");

            migrationBuilder.DropColumn(
                name: "PropertyId1",
                table: "Viewing");

            migrationBuilder.CreateIndex(
                name: "IX_Viewing_BuyerId",
                table: "Viewing",
                column: "BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Viewing_Customer_BuyerId",
                table: "Viewing",
                column: "BuyerId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Viewing_Customer_BuyerId",
                table: "Viewing");

            migrationBuilder.DropForeignKey(
                name: "FK_Viewing_Property_PropertyId",
                table: "Viewing");

            migrationBuilder.DropIndex(
                name: "IX_Viewing_BuyerId",
                table: "Viewing");

            migrationBuilder.AddColumn<long>(
                name: "PropertyId1",
                table: "Viewing",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Viewing_PropertyId1",
                table: "Viewing",
                column: "PropertyId1");

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
    }
}
