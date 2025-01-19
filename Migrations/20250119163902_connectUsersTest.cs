using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrpcService1.Migrations
{
    /// <inheritdoc />
    public partial class connectUsersTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityId",
                table: "Users",
                column: "IdentityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AspNetUsers_IdentityId",
                table: "Users",
                column: "IdentityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AspNetUsers_IdentityId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdentityId",
                table: "Users");
        }
    }
}
