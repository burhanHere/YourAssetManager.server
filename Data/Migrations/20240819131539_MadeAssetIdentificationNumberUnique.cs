using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourAssetManager.server.Data.Migrations
{
    /// <inheritdoc />
    public partial class MadeAssetIdentificationNumberUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetIdentificationNumber",
                table: "Assets",
                column: "AssetIdentificationNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetIdentificationNumber",
                table: "Assets");
        }
    }
}
