using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourAssetManager.server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAssettable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpecialFeatures",
                table: "AssetCategories",
                newName: "RelaventInputFields");

            migrationBuilder.AddColumn<int>(
                name: "AssetSubCategoryId",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetSubCategoryId",
                table: "Assets",
                column: "AssetSubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetSubCategories_AssetSubCategoryId",
                table: "Assets",
                column: "AssetSubCategoryId",
                principalTable: "AssetSubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetSubCategories_AssetSubCategoryId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetSubCategoryId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetSubCategoryId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "RelaventInputFields",
                table: "AssetCategories",
                newName: "SpecialFeatures");
        }
    }
}
