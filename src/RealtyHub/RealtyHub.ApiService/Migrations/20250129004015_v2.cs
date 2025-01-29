using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RealtyHub.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractContent");

            migrationBuilder.CreateTable(
                name: "ContractTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ShowInPage = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTemplate", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ContractTemplate",
                columns: new[] { "Id", "Extension", "Name", "ShowInPage", "Type" },
                values: new object[,]
                {
                    { "2824aec3-3219-4d81-a97a-c3b80ca72844", ".pdf", "Modelo Padrão", true, 0 },
                    { "2f4c556d-6850-4b3d-afe9-d7c2bd282718", ".docx", "Modelo de Contrato - PFPJ", false, 3 },
                    { "a2c16556-5098-4496-ae7a-1f9b6d0e8fcf", ".docx", "Modelo de Contrato - PJPJ", false, 1 },
                    { "f7581a63-f4f0-4881-b6ed-6a4100b4182e", ".docx", "Modelo de Contrato - PFPF", false, 2 },
                    { "fd7ed50d-8f42-4288-8877-3cb8095370e7", ".docx", "Modelo de Contrato - PJPF", false, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractTemplate");

            migrationBuilder.CreateTable(
                name: "ContractContent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShowInPage = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractContent", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ContractContent",
                columns: new[] { "Id", "Extension", "IsActive", "Name", "ShowInPage", "Type", "UserId" },
                values: new object[,]
                {
                    { "2824aec3-3219-4d81-a97a-c3b80ca72844", ".pdf", true, "Modelo Padrão", true, 0, "" },
                    { "2f4c556d-6850-4b3d-afe9-d7c2bd282718", ".docx", true, "Modelo de Contrato - PFPJ", false, 3, "" },
                    { "a2c16556-5098-4496-ae7a-1f9b6d0e8fcf", ".docx", true, "Modelo de Contrato - PJPJ", false, 1, "" },
                    { "f7581a63-f4f0-4881-b6ed-6a4100b4182e", ".docx", true, "Modelo de Contrato - PFPF", false, 2, "" },
                    { "fd7ed50d-8f42-4288-8877-3cb8095370e7", ".docx", true, "Modelo de Contrato - PJPF", false, 4, "" }
                });
        }
    }
}
