using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtyHub.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Offer_OfferId",
                table: "Payment");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Offer_OfferId",
                table: "Payment",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Offer_OfferId",
                table: "Payment");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Offer_OfferId",
                table: "Payment",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
