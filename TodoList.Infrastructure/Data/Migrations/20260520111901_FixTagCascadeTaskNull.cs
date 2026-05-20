using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTagCascadeTaskNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AspNetUsers_IdUser",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_IdUser",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "Tags");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                table: "Tags",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AspNetUsers_UserId",
                table: "Tags",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AspNetUsers_UserId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UserId",
                table: "Tags");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "IdUser",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_IdUser",
                table: "Tags",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AspNetUsers_IdUser",
                table: "Tags",
                column: "IdUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
