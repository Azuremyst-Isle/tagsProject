using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.data.migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToRfidTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_item_rfid_tag",
                table: "item",
                column: "rfid_tag",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_item_rfid_tag", table: "item");
        }
    }
}
