using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateInstaceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Instances");

            migrationBuilder.AlterColumn<string>(
                name: "Host",
                table: "Instances",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApiKeyCreatedAt",
                table: "Instances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ApiKeyLastUsedAt",
                table: "Instances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationName",
                table: "Instances",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Instances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Instances",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Environment",
                table: "Instances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Instances",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "LogPath",
                table: "Instances",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Instances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instances_ApplicationName",
                table: "Instances",
                column: "ApplicationName");

            migrationBuilder.CreateIndex(
                name: "IX_Instances_Environment",
                table: "Instances",
                column: "Environment");

            migrationBuilder.CreateIndex(
                name: "IX_Instances_Host",
                table: "Instances",
                column: "Host");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instances_ApplicationName",
                table: "Instances");

            migrationBuilder.DropIndex(
                name: "IX_Instances_Environment",
                table: "Instances");

            migrationBuilder.DropIndex(
                name: "IX_Instances_Host",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "ApiKeyCreatedAt",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "ApiKeyLastUsedAt",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "ApplicationName",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "Environment",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "LogPath",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Instances");

            migrationBuilder.AlterColumn<string>(
                name: "Host",
                table: "Instances",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Instances",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
