using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.data.migrations
{
    /// <inheritdoc />
    public partial class RenameItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_item",
                table: "item");

            migrationBuilder.RenameTable(
                name: "item",
                newName: "new_table_name");

            migrationBuilder.RenameIndex(
                name: "IX_item_rfid_tag",
                table: "new_table_name",
                newName: "IX_new_table_name_rfid_tag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_new_table_name",
                table: "new_table_name",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_new_table_name",
                table: "new_table_name");

            migrationBuilder.RenameTable(
                name: "new_table_name",
                newName: "item");

            migrationBuilder.RenameIndex(
                name: "IX_new_table_name_rfid_tag",
                table: "item",
                newName: "IX_item_rfid_tag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_item",
                table: "item",
                column: "Id");
        }
    }
}
