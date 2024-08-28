using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourAssetManager.server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedOrganizationTableAddedConfirmationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OrganizationConfirmed",
                table: "Organizations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizationConfirmed",
                table: "Organizations");
        }
    }
}
