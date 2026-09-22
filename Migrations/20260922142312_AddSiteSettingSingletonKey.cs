using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeSure.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSettingSingletonKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SingletonKey",
                table: "SiteSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SiteSettings_SingletonKey",
                table: "SiteSettings",
                column: "SingletonKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SiteSettings_SingletonKey",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "SingletonKey",
                table: "SiteSettings");
        }
    }
}
