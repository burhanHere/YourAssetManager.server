using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourAssetManager.server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAssetStatusSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AssetStatuses",
                keyColumn: "Id",
                keyValue: 5,
                column: "StatusName",
                value: "Available");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AssetStatuses",
                keyColumn: "Id",
                keyValue: 5,
                column: "StatusName",
                value: "Idle");
        }
    }
}
