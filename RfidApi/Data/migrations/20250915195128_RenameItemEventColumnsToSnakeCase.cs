using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfidApi.Data.migrations
{
    /// <inheritdoc />
    public partial class RenameItemEventColumnsToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "IX_Users_email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ItemEvents",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "ItemEvents",
                newName: "item_id");

            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "ItemEvents",
                newName: "event_type");

            migrationBuilder.RenameColumn(
                name: "EventPayload",
                table: "ItemEvents",
                newName: "event_payload");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ItemEvents",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_ItemEvents_ItemId",
                table: "ItemEvents",
                newName: "IX_ItemEvents_item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Users_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ItemEvents",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "item_id",
                table: "ItemEvents",
                newName: "ItemId");

            migrationBuilder.RenameColumn(
                name: "event_type",
                table: "ItemEvents",
                newName: "EventType");

            migrationBuilder.RenameColumn(
                name: "event_payload",
                table: "ItemEvents",
                newName: "EventPayload");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ItemEvents",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ItemEvents_item_id",
                table: "ItemEvents",
                newName: "IX_ItemEvents_ItemId");
        }
    }
}
