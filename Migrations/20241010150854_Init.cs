using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace videohallen_gherghett.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamePublishers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePublishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentals_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rentables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Game_Title = table.Column<string>(type: "TEXT", nullable: true),
                    PublisherId = table.Column<int>(type: "INTEGER", nullable: true),
                    Game_ReleaseDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Model = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentables_GamePublishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "GamePublishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Returns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RentalId = table.Column<int>(type: "INTEGER", nullable: false)
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
                name: "Copies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RentableId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Copies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Copies_Rentables_RentableId",
                        column: x => x.RentableId,
                        principalTable: "Rentables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieMovieGenre",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "INTEGER", nullable: false),
                    MoviesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieMovieGenre", x => new { x.GenresId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_MovieMovieGenre_MovieGenres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "MovieGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieMovieGenre_Rentables_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Rentables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Copies_RentableId",
                table: "Copies",
                column: "RentableId");

            migrationBuilder.CreateIndex(
                name: "IX_CopyRental_RentedCopiesId",
                table: "CopyRental",
                column: "RentedCopiesId");

            migrationBuilder.CreateIndex(
                name: "IX_CopyReturn_ReturnsId",
                table: "CopyReturn",
                column: "ReturnsId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieMovieGenre_MoviesId",
                table: "MovieMovieGenre",
                column: "MoviesId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentables_PublisherId",
                table: "Rentables",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CustomerId",
                table: "Rentals",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_RentalId",
                table: "Returns",
                column: "RentalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CopyRental");

            migrationBuilder.DropTable(
                name: "CopyReturn");

            migrationBuilder.DropTable(
                name: "MovieMovieGenre");

            migrationBuilder.DropTable(
                name: "Copies");

            migrationBuilder.DropTable(
                name: "Returns");

            migrationBuilder.DropTable(
                name: "MovieGenres");

            migrationBuilder.DropTable(
                name: "Rentables");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "GamePublishers");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
