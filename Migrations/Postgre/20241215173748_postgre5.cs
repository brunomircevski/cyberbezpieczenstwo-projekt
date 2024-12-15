using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDwAS_projekt.Migrations.Postgre
{
    /// <inheritdoc />
    public partial class postgre5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedDays",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FullPrice",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaidPrice",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Payments",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "AddedDays",
                table: "Payments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "Payments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FullPrice",
                table: "Payments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PaidPrice",
                table: "Payments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
