using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourAssetManager.server.Data.Migrations
{
    /// <inheritdoc />
    public partial class MadeAssetIdentificationNumberUniqueAndNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CatagoryReleventFeildsData",
                table: "Assets",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "CatagoryReleventFeildsData",
                keyValue: null,
                column: "CatagoryReleventFeildsData",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CatagoryReleventFeildsData",
                table: "Assets",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
