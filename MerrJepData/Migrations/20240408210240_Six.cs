using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerrJepData.Migrations
{
    /// <inheritdoc />
    public partial class Six : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Items",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ApplicationUserId",
                table: "Items",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_AspNetUsers_ApplicationUserId",
                table: "Items",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_AspNetUsers_ApplicationUserId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ApplicationUserId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Items");
        }
    }
}
