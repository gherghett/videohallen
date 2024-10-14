﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VideoHallen;

#nullable disable

namespace videohallen_gherghett.Migrations
{
    [DbContext(typeof(VideoHallDbContext))]
    [Migration("20241011204711_PaidFees")]
    partial class PaidFees
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("MovieMovieGenre", b =>
                {
                    b.Property<int>("GenresId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MoviesId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GenresId", "MoviesId");

                    b.HasIndex("MoviesId");

                    b.ToTable("MovieMovieGenre");
                });

            modelBuilder.Entity("VideoHallen.Models.Copy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Damaged")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Out")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RentableId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Unusable")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RentableId");

                    b.ToTable("Copies");
                });

            modelBuilder.Entity("VideoHallen.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("VideoHallen.Models.Fine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Paid")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RentedCopyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("RentedCopyId")
                        .IsUnique();

                    b.ToTable("Fines");
                });

            modelBuilder.Entity("VideoHallen.Models.GamePublisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("GamePublishers");
                });

            modelBuilder.Entity("VideoHallen.Models.MovieGenre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MovieGenres");
                });

            modelBuilder.Entity("VideoHallen.Models.Rentable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Rentables");

                    b.HasDiscriminator().HasValue("Rentable");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("VideoHallen.Models.Rental", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Complete")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Rentals");
                });

            modelBuilder.Entity("VideoHallen.Models.RentedCopy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CopyId")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("DueByDate")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<int>("RentalId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ReturnDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CopyId");

                    b.HasIndex("RentalId");

                    b.ToTable("RentedCopys");
                });

            modelBuilder.Entity("VideoHallen.Models.Game", b =>
                {
                    b.HasBaseType("VideoHallen.Models.Rentable");

                    b.Property<int>("PublisherId")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("ReleaseDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasIndex("PublisherId");

                    b.ToTable("Rentables", t =>
                        {
                            t.Property("ReleaseDate")
                                .HasColumnName("Game_ReleaseDate");

                            t.Property("Title")
                                .HasColumnName("Game_Title");
                        });

                    b.HasDiscriminator().HasValue("Game");
                });

            modelBuilder.Entity("VideoHallen.Models.Movie", b =>
                {
                    b.HasBaseType("VideoHallen.Models.Rentable");

                    b.Property<DateOnly>("ReleaseDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("Movie");
                });

            modelBuilder.Entity("VideoHallen.Models.RentConsole", b =>
                {
                    b.HasBaseType("VideoHallen.Models.Rentable");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("RentConsole");
                });

            modelBuilder.Entity("MovieMovieGenre", b =>
                {
                    b.HasOne("VideoHallen.Models.MovieGenre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VideoHallen.Models.Movie", null)
                        .WithMany()
                        .HasForeignKey("MoviesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VideoHallen.Models.Copy", b =>
                {
                    b.HasOne("VideoHallen.Models.Rentable", "Rentable")
                        .WithMany("Copies")
                        .HasForeignKey("RentableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rentable");
                });

            modelBuilder.Entity("VideoHallen.Models.Fine", b =>
                {
                    b.HasOne("VideoHallen.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VideoHallen.Models.RentedCopy", "RentedCopy")
                        .WithOne("Fine")
                        .HasForeignKey("VideoHallen.Models.Fine", "RentedCopyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("RentedCopy");
                });

            modelBuilder.Entity("VideoHallen.Models.Rental", b =>
                {
                    b.HasOne("VideoHallen.Models.Customer", "Customer")
                        .WithMany("Rentals")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("VideoHallen.Models.RentedCopy", b =>
                {
                    b.HasOne("VideoHallen.Models.Copy", "Copy")
                        .WithMany("RentedCopys")
                        .HasForeignKey("CopyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VideoHallen.Models.Rental", "Rental")
                        .WithMany("RentedCopys")
                        .HasForeignKey("RentalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Copy");

                    b.Navigation("Rental");
                });

            modelBuilder.Entity("VideoHallen.Models.Game", b =>
                {
                    b.HasOne("VideoHallen.Models.GamePublisher", "Publisher")
                        .WithMany("Games")
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("VideoHallen.Models.Copy", b =>
                {
                    b.Navigation("RentedCopys");
                });

            modelBuilder.Entity("VideoHallen.Models.Customer", b =>
                {
                    b.Navigation("Rentals");
                });

            modelBuilder.Entity("VideoHallen.Models.GamePublisher", b =>
                {
                    b.Navigation("Games");
                });

            modelBuilder.Entity("VideoHallen.Models.Rentable", b =>
                {
                    b.Navigation("Copies");
                });

            modelBuilder.Entity("VideoHallen.Models.Rental", b =>
                {
                    b.Navigation("RentedCopys");
                });

            modelBuilder.Entity("VideoHallen.Models.RentedCopy", b =>
                {
                    b.Navigation("Fine");
                });
#pragma warning restore 612, 618
        }
    }
}
