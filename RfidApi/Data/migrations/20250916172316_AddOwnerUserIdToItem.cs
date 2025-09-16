using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.Data.migrations
{
    /// <inheritdoc />
    public partial class AddOwnerUserIdToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerUserId",
                table: "TagItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagItems_OwnerUserId",
                table: "TagItems",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TagItems_Users_OwnerUserId",
                table: "TagItems",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagItems_Users_OwnerUserId",
                table: "TagItems");

            migrationBuilder.DropIndex(
                name: "IX_TagItems_OwnerUserId",
                table: "TagItems");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "TagItems");
        }
    }
}
