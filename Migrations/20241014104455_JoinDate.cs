using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace videohallen_gherghett.Migrations
{
    /// <inheritdoc />
    public partial class JoinDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "JoinDate",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinDate",
                table: "Customers");
        }
    }
}
