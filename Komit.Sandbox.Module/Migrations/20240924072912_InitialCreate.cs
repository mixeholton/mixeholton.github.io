using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Komit.Sandbox.Module.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Sandbox");

            migrationBuilder.CreateTable(
                name: "Boards",
                schema: "Sandbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cycle",
                schema: "Sandbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cycle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Sandbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Test = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wine",
                schema: "Sandbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkItemState",
                schema: "Sandbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemState_Boards_BoardId",
                        column: x => x.BoardId,
                        principalSchema: "Sandbox",
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestSubItemState",
                schema: "Sandbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSubItemState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSubItemState_Users_TestId",
                        column: x => x.TestId,
                        principalSchema: "Sandbox",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boards_TenantId",
                schema: "Sandbox",
                table: "Boards",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Cycle_TenantId",
                schema: "Sandbox",
                table: "Cycle",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSubItemState_TenantId",
                schema: "Sandbox",
                table: "TestSubItemState",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSubItemState_TestId",
                schema: "Sandbox",
                table: "TestSubItemState",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                schema: "Sandbox",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Wine_TenantId",
                schema: "Sandbox",
                table: "Wine",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemState_BoardId",
                schema: "Sandbox",
                table: "WorkItemState",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemState_TenantId",
                schema: "Sandbox",
                table: "WorkItemState",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cycle",
                schema: "Sandbox");

            migrationBuilder.DropTable(
                name: "TestSubItemState",
                schema: "Sandbox");

            migrationBuilder.DropTable(
                name: "Wine",
                schema: "Sandbox");

            migrationBuilder.DropTable(
                name: "WorkItemState",
                schema: "Sandbox");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Sandbox");

            migrationBuilder.DropTable(
                name: "Boards",
                schema: "Sandbox");
        }
    }
}
