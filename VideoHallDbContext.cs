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
    //public DbSet<Return> Returns { get; set; }
    public DbSet<RentedCopy> RentedCopys { get; set; }
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
            
            // customers.Property(c => c.OutstandingFines) //funkar inte med sqlite
            // .HasComputedColumnSql("SELECT SUM(Amount) FROM Fines WHERE CustomerId = Id");
        });

        modelBuilder.Entity<Rental>(rentals =>
        {
            rentals.HasMany(r => r.RentedCopys)
                .WithOne(rentable => rentable.Rental)
                .HasForeignKey(rentable => rentable.RentalId);
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

        modelBuilder.Entity<Fine>()
            .HasOne(f => f.Customer);
        
        modelBuilder.Entity<Fine>()
            .HasOne(f => f.RentedCopy)
            .WithMany(re => re.Fines);
    }
}