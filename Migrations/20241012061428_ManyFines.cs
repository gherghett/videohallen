using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace videohallen_gherghett.Migrations
{
    /// <inheritdoc />
    public partial class ManyFines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Fines_RentedCopyId",
                table: "Fines");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_RentedCopyId",
                table: "Fines",
                column: "RentedCopyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Fines_RentedCopyId",
                table: "Fines");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_RentedCopyId",
                table: "Fines",
                column: "RentedCopyId",
                unique: true);
        }
    }
}
