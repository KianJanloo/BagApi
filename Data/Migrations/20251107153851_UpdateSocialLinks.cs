using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BagApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSocialLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "SocialLinks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "SocialLinks");
        }
    }
}
