using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Funeraria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FILIAIS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NUMERO = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILIAIS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SERVICOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false),
                    ATIVO = table.Column<int>(type: "NUMBER(1)", nullable: false),
                    TIPO_SERVICO = table.Column<string>(type: "NVARCHAR2(21)", maxLength: 21, nullable: false),
                    MENSALIDADE_BASE = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: true),
                    CARENCIA_MAX_MESES = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    COBERTURA_MAXIMA = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    TAXA_PACOTE_BASICO = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: true),
                    TAXA_PACOTE_PREMIUM = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: true),
                    TAXA_DESLOCAMENTO = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    CNPJ_CEMITERIO = table.Column<string>(type: "NVARCHAR2(18)", maxLength: 18, nullable: true),
                    VALOR_PERPETUIDADE = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SERVICOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CLIENTES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CRIADO_EM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FILIAL_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TIPO_CLIENTE = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: true),
                    DATA_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    RENDA_MENSAL = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    CNPJ = table.Column<string>(type: "NVARCHAR2(18)", maxLength: 18, nullable: true),
                    RAZAO_SOCIAL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    FATURAMENTO_MENSAL = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENTES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CLIENTES_FILIAIS_FILIAL_ID",
                        column: x => x.FILIAL_ID,
                        principalTable: "FILIAIS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONTRATACOES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CLIENTE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SERVICO_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VALOR_SOLICITADO = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    PRAZO_MESES = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    STATUS = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MENSAGEM = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    SCORE_CALCULADO = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TAXA_APLICADA = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: true),
                    SOLICITADA_EM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PROCESSADA_EM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTRATACOES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CONTRATACOES_CLIENTES_CLIENTE_ID",
                        column: x => x.CLIENTE_ID,
                        principalTable: "CLIENTES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONTRATACOES_SERVICOS_SERVICO_ID",
                        column: x => x.SERVICO_ID,
                        principalTable: "SERVICOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FILIAIS_NUMERO",
                table: "FILIAIS",
                column: "NUMERO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTES_FILIAL_ID",
                table: "CLIENTES",
                column: "FILIAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTES_CNPJ",
                table: "CLIENTES",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTES_CPF",
                table: "CLIENTES",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CONTRATACOES_CLIENTE_ID",
                table: "CONTRATACOES",
                column: "CLIENTE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRATACOES_SERVICO_ID",
                table: "CONTRATACOES",
                column: "SERVICO_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONTRATACOES");

            migrationBuilder.DropTable(
                name: "CLIENTES");

            migrationBuilder.DropTable(
                name: "SERVICOS");

            migrationBuilder.DropTable(
                name: "FILIAIS");
        }
    }
}
