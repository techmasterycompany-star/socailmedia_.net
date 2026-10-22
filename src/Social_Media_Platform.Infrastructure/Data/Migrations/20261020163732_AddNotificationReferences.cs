using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Platform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_recipient_id",
                table: "notifications");

            migrationBuilder.AddColumn<long>(
                name: "comment_id",
                table: "notifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "follow_id",
                table: "notifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "live_stream_id",
                table: "notifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "post_id",
                table: "notifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_comment_id",
                table: "notifications",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_follow_id",
                table: "notifications",
                column: "follow_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_live_stream_id",
                table: "notifications",
                column: "live_stream_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_post_id",
                table: "notifications",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_recipient_id_actor_id_notification_type_follow_id_post_id_comment_id_live_stream_id",
                table: "notifications",
                columns: new[] { "recipient_id", "actor_id", "notification_type", "follow_id", "post_id", "comment_id", "live_stream_id" });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_recipient_id_is_read_created_at",
                table: "notifications",
                columns: new[] { "recipient_id", "is_read", "created_at" });

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_comments_comment_id",
                table: "notifications",
                column: "comment_id",
                principalTable: "comments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_follows_follow_id",
                table: "notifications",
                column: "follow_id",
                principalTable: "follows",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_live_streams_live_stream_id",
                table: "notifications",
                column: "live_stream_id",
                principalTable: "live_streams",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_posts_post_id",
                table: "notifications",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_comments_comment_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_follows_follow_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_live_streams_live_stream_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_posts_post_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_comment_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_follow_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_live_stream_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_post_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_recipient_id_actor_id_notification_type_follow_id_post_id_comment_id_live_stream_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_recipient_id_is_read_created_at",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "comment_id",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "follow_id",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "live_stream_id",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "post_id",
                table: "notifications");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_recipient_id",
                table: "notifications",
                column: "recipient_id");
        }
    }
}
