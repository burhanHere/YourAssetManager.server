using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourAssetManager.server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCatagoryOrganizationIdTOCatagorytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatagoryOrganizationId",
                table: "AssetCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_CatagoryOrganizationId",
                table: "AssetCategories",
                column: "CatagoryOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategories_Organizations_CatagoryOrganizationId",
                table: "AssetCategories",
                column: "CatagoryOrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategories_Organizations_CatagoryOrganizationId",
                table: "AssetCategories");

            migrationBuilder.DropIndex(
                name: "IX_AssetCategories_CatagoryOrganizationId",
                table: "AssetCategories");

            migrationBuilder.DropColumn(
                name: "CatagoryOrganizationId",
                table: "AssetCategories");
        }
    }
}
