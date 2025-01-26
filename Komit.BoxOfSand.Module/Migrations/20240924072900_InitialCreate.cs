using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Komit.BoxOfSand.Module.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "BoxOfSand");

            migrationBuilder.CreateTable(
                name: "Books",
                schema: "BoxOfSand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Collection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Boxes",
                schema: "BoxOfSand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentState",
                schema: "BoxOfSand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentState_Boxes_BoxId",
                        column: x => x.BoxId,
                        principalSchema: "BoxOfSand",
                        principalTable: "Boxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_TenantId",
                schema: "BoxOfSand",
                table: "Books",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_TenantId",
                schema: "BoxOfSand",
                table: "Boxes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentState_BoxId",
                schema: "BoxOfSand",
                table: "ContentState",
                column: "BoxId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentState_TenantId",
                schema: "BoxOfSand",
                table: "ContentState",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books",
                schema: "BoxOfSand");

            migrationBuilder.DropTable(
                name: "ContentState",
                schema: "BoxOfSand");

            migrationBuilder.DropTable(
                name: "Boxes",
                schema: "BoxOfSand");
        }
    }
}
