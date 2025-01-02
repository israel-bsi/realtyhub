using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtyHub.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ViewingStatus",
                table: "Viewing",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Creci",
                table: "IdentityUser",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Creci",
                table: "IdentityUser");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Viewing",
                newName: "ViewingStatus");
        }
    }
}
