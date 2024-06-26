﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerrJepData.Migrations
{
    /// <inheritdoc />
    public partial class AddedInvalidatedToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Invalidated",
                table: "Items",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Invalidated",
                table: "Items");
        }
    }
}
