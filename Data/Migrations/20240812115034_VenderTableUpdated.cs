using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourAssetManager.server.Data.Migrations
{
    /// <inheritdoc />
    public partial class VenderTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Venders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Venders_OrganizationId",
                table: "Venders",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Venders_Organizations_OrganizationId",
                table: "Venders",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Venders_Organizations_OrganizationId",
                table: "Venders");

            migrationBuilder.DropIndex(
                name: "IX_Venders_OrganizationId",
                table: "Venders");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Venders");
        }
    }
}
