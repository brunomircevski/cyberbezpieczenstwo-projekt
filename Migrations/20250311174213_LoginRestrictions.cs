using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cyberbezpieczenstwo.Migrations
{
    /// <inheritdoc />
    public partial class LoginRestrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginsCounter",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFailedLogin",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxFailedLogins",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedLoginsCounter",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastFailedLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MaxFailedLogins",
                table: "Users");
        }
    }
}
