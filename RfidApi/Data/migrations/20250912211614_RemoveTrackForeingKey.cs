using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.Data.migrations
{
    /// <inheritdoc />
    public partial class RemoveTrackForeingKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemEvents_TagItems_ItemId",
                table: "ItemEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_ItemEvents_TagItems_ItemId",
                table: "ItemEvents",
                column: "ItemId",
                principalTable: "TagItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
