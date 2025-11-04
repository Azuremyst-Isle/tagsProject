using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.Data.migrations
{
    /// <inheritdoc />
    public partial class UpdateRetailerIdB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_Retailers_RetailerId",
                table: "items");

            migrationBuilder.AddForeignKey(
                name: "FK_items_Retailers_RetailerId",
                table: "items",
                column: "RetailerId",
                principalTable: "Retailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_Retailers_RetailerId",
                table: "items");

            migrationBuilder.AddForeignKey(
                name: "FK_items_Retailers_RetailerId",
                table: "items",
                column: "RetailerId",
                principalTable: "Retailers",
                principalColumn: "Id");
        }
    }
}
