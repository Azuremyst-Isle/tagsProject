using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.Data.migrations
{
    /// <inheritdoc />
    public partial class AddRfidTagToItemEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "rfid_tag",
                table: "ItemEvents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rfid_tag",
                table: "ItemEvents");
        }
    }
}
