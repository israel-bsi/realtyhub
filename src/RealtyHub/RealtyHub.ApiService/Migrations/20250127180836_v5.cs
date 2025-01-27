using System;
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
                name: "Content",
                table: "Contract",
                newName: "FileId");

            migrationBuilder.AddColumn<string>(
                name: "IssuingAuthority",
                table: "Customer",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RgIssueDate",
                table: "Customer",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuingAuthority",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "RgIssueDate",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "Contract",
                newName: "Content");
        }
    }
}
