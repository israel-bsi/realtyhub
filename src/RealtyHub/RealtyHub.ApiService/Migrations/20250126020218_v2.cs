using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtyHub.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Customer_CustomerId",
                table: "Offer");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Offer",
                newName: "BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_Offer_CustomerId",
                table: "Offer",
                newName: "IX_Offer_BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Customer_BuyerId",
                table: "Offer",
                column: "BuyerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Customer_BuyerId",
                table: "Offer");

            migrationBuilder.RenameColumn(
                name: "BuyerId",
                table: "Offer",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Offer_BuyerId",
                table: "Offer",
                newName: "IX_Offer_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Customer_CustomerId",
                table: "Offer",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
