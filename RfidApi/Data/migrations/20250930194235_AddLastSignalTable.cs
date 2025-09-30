using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.Data.migrations
{
    /// <inheritdoc />
    public partial class AddLastSignalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_signal",
                table: "items",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_signal",
                table: "items");
        }
    }
}
