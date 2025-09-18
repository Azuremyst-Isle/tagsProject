using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.Data.migrations
{
    /// <inheritdoc />
    public partial class RenameTagItemsToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagItems_Users_OwnerUserId",
                table: "TagItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagItems",
                table: "TagItems");

            migrationBuilder.RenameTable(
                name: "TagItems",
                newName: "items");

            migrationBuilder.RenameIndex(
                name: "IX_TagItems_rfid_tag",
                table: "items",
                newName: "IX_items_rfid_tag");

            migrationBuilder.RenameIndex(
                name: "IX_TagItems_OwnerUserId",
                table: "items",
                newName: "IX_items_OwnerUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_items",
                table: "items",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_items_Users_OwnerUserId",
                table: "items",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_Users_OwnerUserId",
                table: "items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_items",
                table: "items");

            migrationBuilder.RenameTable(
                name: "items",
                newName: "TagItems");

            migrationBuilder.RenameIndex(
                name: "IX_items_rfid_tag",
                table: "TagItems",
                newName: "IX_TagItems_rfid_tag");

            migrationBuilder.RenameIndex(
                name: "IX_items_OwnerUserId",
                table: "TagItems",
                newName: "IX_TagItems_OwnerUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagItems",
                table: "TagItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagItems_Users_OwnerUserId",
                table: "TagItems",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
