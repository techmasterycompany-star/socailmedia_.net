using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Platform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class postHiddenStatusUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "posts");
        }
    }
}
