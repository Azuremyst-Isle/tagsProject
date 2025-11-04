using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.data.migrations
{
    /// <inheritdoc />
    public partial class CorrectTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_new_table_name",
                table: "new_table_name");

            migrationBuilder.RenameTable(
                name: "new_table_name",
                newName: "TagItems");

            migrationBuilder.RenameIndex(
                name: "IX_new_table_name_rfid_tag",
                table: "TagItems",
                newName: "IX_TagItems_rfid_tag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagItems",
                table: "TagItems",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TagItems",
                table: "TagItems");

            migrationBuilder.RenameTable(
                name: "TagItems",
                newName: "new_table_name");

            migrationBuilder.RenameIndex(
                name: "IX_TagItems_rfid_tag",
                table: "new_table_name",
                newName: "IX_new_table_name_rfid_tag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_new_table_name",
                table: "new_table_name",
                column: "Id");
        }
    }
}
