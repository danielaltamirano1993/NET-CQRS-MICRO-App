using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microservicio.Items.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchemaCleanV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuarioReferencia",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LimiteItems = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItemsPendientes = table.Column<int>(type: "int", nullable: false),
                    ItemsCompletados = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioReferencia", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "ItemTrabajo",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Relevancia = table.Column<byte>(type: "tinyint", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UsuarioAsignado = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTrabajo", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_ItemTrabajo_UsuarioReferencia_UsuarioAsignado",
                        column: x => x.UsuarioAsignado,
                        principalTable: "UsuarioReferencia",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialAsignacion",
                columns: table => new
                {
                    HistorialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTrabajoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialAsignacion", x => x.HistorialId);
                    table.ForeignKey(
                        name: "FK_HistorialAsignacion_ItemTrabajo_ItemTrabajoId",
                        column: x => x.ItemTrabajoId,
                        principalTable: "ItemTrabajo",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialAsignacion_UsuarioReferencia_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "UsuarioReferencia",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialAsignacion_ItemTrabajoId",
                table: "HistorialAsignacion",
                column: "ItemTrabajoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialAsignacion_UsuarioId",
                table: "HistorialAsignacion",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTrabajo_UsuarioAsignado",
                table: "ItemTrabajo",
                column: "UsuarioAsignado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialAsignacion");

            migrationBuilder.DropTable(
                name: "ItemTrabajo");

            migrationBuilder.DropTable(
                name: "UsuarioReferencia");
        }
    }
}
