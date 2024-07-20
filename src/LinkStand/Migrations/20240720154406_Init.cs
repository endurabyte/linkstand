using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkStand.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Target = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AliasEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AliasId = table.Column<string>(type: "text", nullable: false),
                    Ip = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AliasEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AliasEvent_Alias_AliasId",
                        column: x => x.AliasId,
                        principalTable: "Alias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AliasEvent_AliasId",
                table: "AliasEvent",
                column: "AliasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AliasEvent");

            migrationBuilder.DropTable(
                name: "Alias");
        }
    }
}
