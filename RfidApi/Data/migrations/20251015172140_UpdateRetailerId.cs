using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.Data.migrations
{
    /// <inheritdoc />
    public partial class UpdateRetailerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RetailerId",
                table: "items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_RetailerId",
                table: "items",
                column: "RetailerId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_Retailers_RetailerId",
                table: "items",
                column: "RetailerId",
                principalTable: "Retailers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_Retailers_RetailerId",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_RetailerId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "RetailerId",
                table: "items");
        }
    }
}
