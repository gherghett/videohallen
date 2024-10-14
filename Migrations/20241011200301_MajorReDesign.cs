using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace videohallen_gherghett.Migrations
{
    /// <inheritdoc />
    public partial class MajorReDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_Copies_CopyId",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_Fines_Returns_ReturnId",
                table: "Fines");

            migrationBuilder.DropTable(
                name: "CopyRental");

            migrationBuilder.DropTable(
                name: "CopyReturn");

            migrationBuilder.DropTable(
                name: "Returns");

            migrationBuilder.DropIndex(
                name: "IX_Fines_CopyId",
                table: "Fines");

            migrationBuilder.DropIndex(
                name: "IX_Fines_ReturnId",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "RentalPrices",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "CopyId",
                table: "Fines");

            migrationBuilder.RenameColumn(
                name: "RentalTimes",
                table: "Rentals",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "ReturnId",
                table: "Fines",
                newName: "RentedCopyId");

            migrationBuilder.CreateTable(
                name: "RentedCopys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RentalId = table.Column<int>(type: "INTEGER", nullable: false),
                    CopyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DueByDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentedCopys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentedCopys_Copies_CopyId",
                        column: x => x.CopyId,
                        principalTable: "Copies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentedCopys_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fines_RentedCopyId",
                table: "Fines",
                column: "RentedCopyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentedCopys_CopyId",
                table: "RentedCopys",
                column: "CopyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentedCopys_RentalId",
                table: "RentedCopys",
                column: "RentalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_RentedCopys_RentedCopyId",
                table: "Fines",
                column: "RentedCopyId",
                principalTable: "RentedCopys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_RentedCopys_RentedCopyId",
                table: "Fines");

            migrationBuilder.DropTable(
                name: "RentedCopys");

            migrationBuilder.DropIndex(
                name: "IX_Fines_RentedCopyId",
                table: "Fines");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Rentals",
                newName: "RentalTimes");

            migrationBuilder.RenameColumn(
                name: "RentedCopyId",
                table: "Fines",
                newName: "ReturnId");

            migrationBuilder.AddColumn<string>(
                name: "RentalPrices",
                table: "Rentals",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CopyId",
                table: "Fines",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CopyRental",
                columns: table => new
                {
                    RentalsId = table.Column<int>(type: "INTEGER", nullable: false),
                    RentedCopiesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CopyRental", x => new { x.RentalsId, x.RentedCopiesId });
                    table.ForeignKey(
                        name: "FK_CopyRental_Copies_RentedCopiesId",
                        column: x => x.RentedCopiesId,
                        principalTable: "Copies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CopyRental_Rentals_RentalsId",
                        column: x => x.RentalsId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Returns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RentalId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Returns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Returns_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CopyReturn",
                columns: table => new
                {
                    ReturnedCopiesId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReturnsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CopyReturn", x => new { x.ReturnedCopiesId, x.ReturnsId });
                    table.ForeignKey(
                        name: "FK_CopyReturn_Copies_ReturnedCopiesId",
                        column: x => x.ReturnedCopiesId,
                        principalTable: "Copies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CopyReturn_Returns_ReturnsId",
                        column: x => x.ReturnsId,
                        principalTable: "Returns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fines_CopyId",
                table: "Fines",
                column: "CopyId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_ReturnId",
                table: "Fines",
                column: "ReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_CopyRental_RentedCopiesId",
                table: "CopyRental",
                column: "RentedCopiesId");

            migrationBuilder.CreateIndex(
                name: "IX_CopyReturn_ReturnsId",
                table: "CopyReturn",
                column: "ReturnsId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_RentalId",
                table: "Returns",
                column: "RentalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_Copies_CopyId",
                table: "Fines",
                column: "CopyId",
                principalTable: "Copies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_Returns_ReturnId",
                table: "Fines",
                column: "ReturnId",
                principalTable: "Returns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
