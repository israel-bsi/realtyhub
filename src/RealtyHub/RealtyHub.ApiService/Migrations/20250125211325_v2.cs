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
                name: "FK_Offer_Contract_ContractId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_ContractId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Offer");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_OfferId",
                table: "Contract",
                column: "OfferId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Offer_OfferId",
                table: "Contract",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Offer_OfferId",
                table: "Contract");

            migrationBuilder.DropIndex(
                name: "IX_Contract_OfferId",
                table: "Contract");

            migrationBuilder.AddColumn<long>(
                name: "ContractId",
                table: "Offer",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Offer_ContractId",
                table: "Offer",
                column: "ContractId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Contract_ContractId",
                table: "Offer",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
