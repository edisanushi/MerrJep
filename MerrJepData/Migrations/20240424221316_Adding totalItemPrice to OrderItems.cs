using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerrJepData.Migrations
{
    /// <inheritdoc />
    public partial class AddingtotalItemPricetoOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalItemPrice",
                table: "OrderItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalItemPrice",
                table: "OrderItems");
        }
    }
}
