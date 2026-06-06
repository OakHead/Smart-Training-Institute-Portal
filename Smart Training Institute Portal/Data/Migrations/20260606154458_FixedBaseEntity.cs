using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Training_Institute_Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixedBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CourseInstructors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "CourseInstructors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CourseInstructors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CourseInstructors",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "CourseInstructors",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CourseInstructors");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "CourseInstructors");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CourseInstructors");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CourseInstructors");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "CourseInstructors");
        }
    }
}
