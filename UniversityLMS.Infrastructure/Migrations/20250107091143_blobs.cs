using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityLMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class blobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3f0c4d62-0d56-4e4c-8424-d2d575bf08e5"),
                column: "Role",
                value: "Teacher");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3f0c4d62-0d56-4e4c-8424-d2d575bf08e5"),
                column: "Role",
                value: "Faculty");
        }
    }
}
