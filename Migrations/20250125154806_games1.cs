using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrpcService1.Migrations
{
    /// <inheritdoc />
    public partial class games1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    hostId = table.Column<int>(type: "INTEGER", nullable: false),
                    caseId = table.Column<int>(type: "INTEGER", nullable: false),
                    maxPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    isStarted = table.Column<bool>(type: "INTEGER", nullable: false),
                    createdAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Chests_caseId",
                        column: x => x.caseId,
                        principalTable: "Chests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Users_hostId",
                        column: x => x.hostId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    gameId = table.Column<int>(type: "INTEGER", nullable: false),
                    userId = table.Column<int>(type: "INTEGER", nullable: false),
                    isReady = table.Column<bool>(type: "INTEGER", nullable: false),
                    spinResult = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePlayers_Games_gameId",
                        column: x => x.gameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePlayers_Items_spinResult",
                        column: x => x.spinResult,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePlayers_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_gameId",
                table: "GamePlayers",
                column: "gameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_spinResult",
                table: "GamePlayers",
                column: "spinResult");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_userId",
                table: "GamePlayers",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_caseId",
                table: "Games",
                column: "caseId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_hostId",
                table: "Games",
                column: "hostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamePlayers");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
