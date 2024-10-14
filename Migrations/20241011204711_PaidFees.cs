using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace videohallen_gherghett.Migrations
{
    /// <inheritdoc />
    public partial class PaidFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "Fines",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Fines",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Fines");
        }
    }
}
