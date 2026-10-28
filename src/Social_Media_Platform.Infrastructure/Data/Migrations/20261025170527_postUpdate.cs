using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Platform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class postUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_post_media_post_id",
                table: "post_media");

            migrationBuilder.DropColumn(
                name: "is_public",
                table: "posts");

            migrationBuilder.RenameColumn(
                name: "media_url",
                table: "post_media",
                newName: "MediaUrl");

            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "category_id",
                table: "posts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "MediaUrl",
                table: "post_media",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "post_media",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryType",
                table: "post_media",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "post_media",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "post_media",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SizeBytes",
                table: "post_media",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Hashtags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hashtags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedPosts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedPosts_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedPosts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostHashtags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    HashtagId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostHashtags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostHashtags_Hashtags_HashtagId",
                        column: x => x.HashtagId,
                        principalTable: "Hashtags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostHashtags_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_posts_category_id",
                table: "posts",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_created_at_id",
                table: "posts",
                columns: new[] { "created_at", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_post_media_post_id_DisplayOrder",
                table: "post_media",
                columns: new[] { "post_id", "DisplayOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hashtags_NormalizedName",
                table: "Hashtags",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostHashtags_HashtagId",
                table: "PostHashtags",
                column: "HashtagId");

            migrationBuilder.CreateIndex(
                name: "IX_PostHashtags_PostId_HashtagId",
                table: "PostHashtags",
                columns: new[] { "PostId", "HashtagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedPosts_PostId",
                table: "SavedPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedPosts_UserId_PostId",
                table: "SavedPosts",
                columns: new[] { "UserId", "PostId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_categories_category_id",
                table: "posts",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_categories_category_id",
                table: "posts");

            migrationBuilder.DropTable(
                name: "PostHashtags");

            migrationBuilder.DropTable(
                name: "SavedPosts");

            migrationBuilder.DropTable(
                name: "Hashtags");

            migrationBuilder.DropIndex(
                name: "IX_posts_category_id",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_created_at_id",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_post_media_post_id_DisplayOrder",
                table: "post_media");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "post_media");

            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "post_media");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "post_media");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "post_media");

            migrationBuilder.DropColumn(
                name: "SizeBytes",
                table: "post_media");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "post_media",
                newName: "media_url");

            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                table: "posts",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "media_url",
                table: "post_media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_post_media_post_id",
                table: "post_media",
                column: "post_id");
        }
    }
}
