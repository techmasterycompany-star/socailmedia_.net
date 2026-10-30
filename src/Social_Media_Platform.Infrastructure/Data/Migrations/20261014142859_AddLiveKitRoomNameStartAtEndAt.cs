using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Platform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLiveKitRoomNameStartAtEndAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "live_streams",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "audience_setting",
                table: "live_streams",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ended_at",
                table: "live_streams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "room_name",
                table: "live_streams",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "start_at",
                table: "live_streams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_live_streams_room_name",
                table: "live_streams",
                column: "room_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_live_streams_room_name",
                table: "live_streams");

            migrationBuilder.DropColumn(
                name: "ended_at",
                table: "live_streams");

            migrationBuilder.DropColumn(
                name: "room_name",
                table: "live_streams");

            migrationBuilder.DropColumn(
                name: "start_at",
                table: "live_streams");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "live_streams",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "audience_setting",
                table: "live_streams",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20);
        }
    }
}
