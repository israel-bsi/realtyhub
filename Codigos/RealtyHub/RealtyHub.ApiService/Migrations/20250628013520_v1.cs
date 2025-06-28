using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RealtyHub.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    NumeroDocumento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Ocupacao = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Nacionalidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    TipoPessoa = table.Column<int>(type: "integer", nullable: false),
                    TipoCliente = table.Column<int>(type: "integer", nullable: false),
                    Logradouro = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Cidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Estado = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Pais = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Cep = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Complemento = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Rg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AutoridadeEmissora = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DataEmissaoRg = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NomeFantasia = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    TipoStatusCivil = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Condominio",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Logradouro = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Cidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Estado = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Pais = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Cep = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Complemento = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Unidades = table.Column<int>(type: "integer", nullable: false),
                    Andares = table.Column<int>(type: "integer", nullable: false),
                    PossuiElevador = table.Column<bool>(type: "boolean", nullable: false),
                    PossuiPiscina = table.Column<bool>(type: "boolean", nullable: false),
                    PossuiSalaoFesta = table.Column<bool>(type: "boolean", nullable: false),
                    PossuiPlayground = table.Column<bool>(type: "boolean", nullable: false),
                    PossuiAcademia = table.Column<bool>(type: "boolean", nullable: false),
                    TaxaCondominial = table.Column<decimal>(type: "numeric", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condominio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ClaimValue = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GivenName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Creci = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    Email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModeloContrato",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Extensao = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    MostrarNaHome = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeloContrato", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Imovel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendedorId = table.Column<long>(type: "bigint", nullable: false),
                    CondominioId = table.Column<long>(type: "bigint", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false),
                    TipoImovel = table.Column<int>(type: "integer", nullable: false),
                    Quarto = table.Column<int>(type: "integer", nullable: false),
                    Banheiro = table.Column<int>(type: "integer", nullable: false),
                    Garagem = table.Column<int>(type: "integer", nullable: false),
                    Area = table.Column<double>(type: "double precision", nullable: false),
                    DetalhesTransacao = table.Column<string>(type: "text", nullable: false),
                    Logradouro = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Cidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Estado = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Pais = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Cep = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Complemento = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Novo = table.Column<bool>(type: "boolean", nullable: false),
                    NumeroRegistro = table.Column<string>(type: "text", nullable: false),
                    RegistroCartorio = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    ExibirNaHome = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imovel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imovel_Cliente_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Imovel_Condominio_CondominioId",
                        column: x => x.CondominioId,
                        principalTable: "Condominio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ClaimValue = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityClaim_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRole_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserRole",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRole_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserToken",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_IdentityUserToken_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FotoImovel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Extensao = table.Column<string>(type: "text", nullable: false),
                    Miniatura = table.Column<bool>(type: "boolean", nullable: false),
                    ImovelId = table.Column<long>(type: "bigint", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotoImovel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FotoImovel_Imovel_ImovelId",
                        column: x => x.ImovelId,
                        principalTable: "Imovel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proposta",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    ImovelId = table.Column<long>(type: "bigint", nullable: false),
                    CompradorId = table.Column<long>(type: "bigint", nullable: false),
                    DataDeEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusProposta = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proposta_Cliente_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proposta_Imovel_ImovelId",
                        column: x => x.ImovelId,
                        principalTable: "Imovel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visita",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataVisita = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusVisita = table.Column<int>(type: "integer", nullable: false),
                    CompradorId = table.Column<long>(type: "bigint", nullable: false),
                    ImovelId = table.Column<long>(type: "bigint", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visita", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visita_Cliente_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visita_Imovel_ImovelId",
                        column: x => x.ImovelId,
                        principalTable: "Imovel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contrato",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendedorId = table.Column<long>(type: "bigint", nullable: false),
                    CompradorId = table.Column<long>(type: "bigint", nullable: false),
                    PropostaId = table.Column<long>(type: "bigint", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataVigencia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataTermino = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAssinatura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArquivoId = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contrato_Cliente_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contrato_Cliente_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contrato_Proposta_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Proposta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagamento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    TipoPagamento = table.Column<int>(type: "integer", nullable: false),
                    Parcelado = table.Column<bool>(type: "boolean", nullable: false),
                    QuantidadeParcelas = table.Column<int>(type: "integer", nullable: false),
                    PropostaId = table.Column<long>(type: "bigint", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamento_Proposta_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Proposta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ModeloContrato",
                columns: new[] { "Id", "Extensao", "MostrarNaHome", "Nome", "Tipo" },
                values: new object[,]
                {
                    { "2824aec3-3219-4d81-a97a-c3b80ca72844", ".pdf", true, "Modelo Padrão", 0 },
                    { "2f4c556d-6850-4b3d-afe9-d7c2bd282718", ".docx", false, "Modelo de Contrato - PFPJ", 3 },
                    { "a2c16556-5098-4496-ae7a-1f9b6d0e8fcf", ".docx", false, "Modelo de Contrato - PJPJ", 1 },
                    { "f7581a63-f4f0-4881-b6ed-6a4100b4182e", ".docx", false, "Modelo de Contrato - PFPF", 2 },
                    { "fd7ed50d-8f42-4288-8877-3cb8095370e7", ".docx", false, "Modelo de Contrato - PJPF", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contrato_CompradorId",
                table: "Contrato",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Contrato_PropostaId",
                table: "Contrato",
                column: "PropostaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contrato_VendedorId",
                table: "Contrato",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_FotoImovel_ImovelId",
                table: "FotoImovel",
                column: "ImovelId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityClaim_UserId",
                table: "IdentityClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRole_NormalizedName",
                table: "IdentityRole",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRole_UserId",
                table: "IdentityRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_NormalizedEmail",
                table: "IdentityUser",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_NormalizedUserName",
                table: "IdentityUser",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserLogin_UserId",
                table: "IdentityUserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Imovel_CondominioId",
                table: "Imovel",
                column: "CondominioId");

            migrationBuilder.CreateIndex(
                name: "IX_Imovel_VendedorId",
                table: "Imovel",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_PropostaId",
                table: "Pagamento",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposta_CompradorId",
                table: "Proposta",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposta_ImovelId",
                table: "Proposta",
                column: "ImovelId");

            migrationBuilder.CreateIndex(
                name: "IX_Visita_CompradorId",
                table: "Visita",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Visita_ImovelId",
                table: "Visita",
                column: "ImovelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contrato");

            migrationBuilder.DropTable(
                name: "FotoImovel");

            migrationBuilder.DropTable(
                name: "IdentityClaim");

            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.DropTable(
                name: "IdentityRoleClaim");

            migrationBuilder.DropTable(
                name: "IdentityUserLogin");

            migrationBuilder.DropTable(
                name: "IdentityUserRole");

            migrationBuilder.DropTable(
                name: "IdentityUserToken");

            migrationBuilder.DropTable(
                name: "ModeloContrato");

            migrationBuilder.DropTable(
                name: "Pagamento");

            migrationBuilder.DropTable(
                name: "Visita");

            migrationBuilder.DropTable(
                name: "IdentityUser");

            migrationBuilder.DropTable(
                name: "Proposta");

            migrationBuilder.DropTable(
                name: "Imovel");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Condominio");
        }
    }
}
