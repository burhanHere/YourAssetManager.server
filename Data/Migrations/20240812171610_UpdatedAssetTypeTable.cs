using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourAssetManager.server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAssetTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "AssetTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_OrganizationId",
                table: "AssetTypes",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTypes_Organizations_OrganizationId",
                table: "AssetTypes",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetTypes_Organizations_OrganizationId",
                table: "AssetTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssetTypes_OrganizationId",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "AssetTypes");
        }
    }
}
