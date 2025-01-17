using System;
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
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Viewing",
                newName: "ViewingStatus");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Viewing",
                newName: "ViewingDate");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ViewingTime",
                table: "Viewing",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewingTime",
                table: "Viewing");

            migrationBuilder.RenameColumn(
                name: "ViewingStatus",
                table: "Viewing",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ViewingDate",
                table: "Viewing",
                newName: "Date");
        }
    }
}
