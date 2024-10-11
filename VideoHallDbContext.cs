using Microsoft.EntityFrameworkCore;
using VideoHallen.Models;

namespace VideoHallen;

public class VideoHallDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Rentable> Rentables { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<RentConsole> RentConsoles { get; set;}
    public DbSet<Copy> Copies { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Return> Returns { get; set; }
    public DbSet<Fine> Fines {get; set;}

    public DbSet<GamePublisher> GamePublishers { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }

    public VideoHallDbContext(DbContextOptions<VideoHallDbContext> options)
        : base(options)
    {
    }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<Customer>()
    //         .HasMany(c => c.Rentals)
    //         .WithOne(r => r.Customer)
    //         .HasForeignKey(r => r.CustomerId);

    //     modelBuilder.Entity<Rental>(rentals =>
    //     {
    //         rentals.HasOne(r => r.Customer);
                
    //         rentals.HasMany(r => r.Rentables);
            
    //         rentals.HasMany(r => r.Returns)
    //             .WithOne(ret => ret.Rental);
    //     });

    //     modelBuilder.Entity<Rentable>()
    //             .HasOne(r => r.RentableType)
    //             .WithMany(t => t.Rentables)
    //             .HasForeignKey(r => r.RentableTypeId);

    //     modelBuilder.Entity<Return>()
    //         .HasMany(r => r.ReturnedItems);
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(customers =>
        {
            customers.HasMany(c => c.Rentals)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId);
        });

        modelBuilder.Entity<Rentable>(rentables =>
        {
            rentables.HasMany(r => r.Copies)
                .WithOne(c => c.Rentable)
                .HasForeignKey(c => c.RentableId);
        });

        modelBuilder.Entity<Movie>()
            .HasMany(r => r.Genres)
            .WithMany(g => g.Movies);
        
        modelBuilder.Entity<Game>(game =>
        {    
            game.HasOne(g => g.Publisher)
            .WithMany(p => p.Games);
        });

        modelBuilder.Entity<Rental>(rentals =>
        {
            rentals.HasMany(r => r.RentedCopies)
                .WithMany(rentable => rentable.Rentals);
            
            rentals.HasMany(r => r.Returns)
                .WithOne(ret => ret.Rental)
                .HasForeignKey(ret => ret.RentalId);
        });

        modelBuilder.Entity<Return>(returns =>
        {
            returns.HasMany(r => r.ReturnedCopies)
                .WithMany(rentable => rentable.Returns);
        });

        modelBuilder.Entity<Fine>()
            .HasOne(f => f.Return)
            .WithMany()
            .HasForeignKey(f => f.ReturnId);

        modelBuilder.Entity<Fine>()
            .HasOne(f => f.Customer)
            .WithMany()
            .HasForeignKey(f => f.CustomerId);
        
        modelBuilder.Entity<Fine>()
            .HasOne(f => f.Copy)
            .WithMany()
            .HasForeignKey(f => f.CopyId);
    }
}