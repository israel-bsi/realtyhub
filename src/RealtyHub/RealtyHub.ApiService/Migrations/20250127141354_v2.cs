using System;
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
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractsContent",
                table: "ContractsContent");

            migrationBuilder.RenameTable(
                name: "ContractsContent",
                newName: "ContractContent");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ContractContent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ContractContent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractContent",
                table: "ContractContent",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractContent",
                table: "ContractContent");

            migrationBuilder.RenameTable(
                name: "ContractContent",
                newName: "ContractsContent");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ContractsContent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ContractsContent",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractsContent",
                table: "ContractsContent",
                column: "Id");
        }
    }
}
