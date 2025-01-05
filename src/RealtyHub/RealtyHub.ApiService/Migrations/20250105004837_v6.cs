using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtyHub.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Offer_OfferId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Viewing");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Offer_OfferId",
                table: "Payment",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Offer_OfferId",
                table: "Payment");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Viewing",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Offer_OfferId",
                table: "Payment",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
