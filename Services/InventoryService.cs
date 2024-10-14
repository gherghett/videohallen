using Microsoft.EntityFrameworkCore;
using VideoHallen;
using VideoHallen.Models;
using VideoHallen.Exceptions;

namespace VideoHallen.Services;
public class InventoryService 
{
    private readonly RentingService _rentingService;
    private readonly VideoHallDbContext _dbContext;
    public InventoryService(VideoHallDbContext context, RentingService rentingService)
    {
        _dbContext = context;
        _rentingService = rentingService;
    }
    //Movie Methods
    public Movie AddMovie(Movie movie, int copies = 1)
    {
        _dbContext.Add(movie);
        _dbContext.SaveChanges();
        AddCopies(movie, copies);
        return movie;
    }

    internal List<Movie> GetAllMovies()
    {
        return _dbContext.Movies.Include(r => r.Copies).ToList();
    }

    // MovieGenre methods
    public MovieGenre AddGenre(string name)
    {
        var genre = new MovieGenre{
            Name = name,
        };
        _dbContext.Add(genre);
        _dbContext.SaveChanges();
        return genre;
    }
    public List<MovieGenre> GetAllMovieGenres()
    {
        return _dbContext.MovieGenres.ToList();
    }

    // Game methods
    public Game AddGame( Game game, int copies)
    {
        _dbContext.Add(game);
        _dbContext.SaveChanges();
        AddCopies(game, copies);
        return game;
    }

    public GamePublisher AddPublisher(string name)
    {
        var publisher = new GamePublisher{Name = name};
        _dbContext.Add(publisher);
        _dbContext.SaveChanges();
        return publisher;
    }
    public List<GamePublisher> GetAllGamePublishers()
    {
        return _dbContext.GamePublishers.ToList();
    }

    // RentConsole methods
    public RentConsole AddRentConsole(string model, int copies)
    {
        RentConsole newConsole = new RentConsole { Model = model };
        _dbContext.Add(newConsole);
        AddCopies(newConsole, copies);
        _dbContext.SaveChanges();
        return newConsole;
    }

    //Copies and Rentable Methods
    public List<Copy> AddCopies(Rentable rentable, int amount = 1)
    {
        List<Copy> copies = new();
        for(int i = 0; i < amount; i++)
        {
            Copy newCopy = new Copy { Rentable = rentable };
            _dbContext.Add(newCopy);
            _dbContext.SaveChanges();
            copies.Add(newCopy);
        }
        return copies;
    }

    public List<Rentable> GetRentables( 
        RentableType types = RentableType.All,
        RentableFlag flags = RentableFlag.All,
        string search = "")
    {
        //return _dbContext.Rentables.Where(r => r is Movie).ToList();
        IQueryable<Rentable> query = _dbContext.Rentables.Include(r => r.Copies);

        query = query.Where(r =>
            (types.HasFlag(RentableType.Movie) && r is Movie) ||
            (types.HasFlag(RentableType.Game) && r is Game) ||
            (types.HasFlag(RentableType.RentConsole) && r is RentConsole)
        );

        List<Rentable> results = new();

        if (flags.HasFlag(RentableFlag.In))
            results.AddRange(query.Where(r => r.Copies.Any(c => !c.Out)));
        if (flags.HasFlag(RentableFlag.Out))
            results.AddRange(query.Where(r => r.Copies.Any(c => c.Out)));
        if (flags.HasFlag(RentableFlag.Damaged))
            results.AddRange(query.Where(r => r.Copies.Any(c => c.Damaged)));
        if (flags.HasFlag(RentableFlag.Destroyed))
            results.AddRange(query.Where(r => r.Copies.Any(c => c.Unusable)));

        //yuck
        return results.Where(r => r.Name().Contains(search, StringComparison.OrdinalIgnoreCase)).Distinct().ToList();
    }

    // Returns the amount of available (not the rented) copies for a Rentable
    public int GetAvailabilityOfRentable(Rentable rentable)
    {
        _dbContext.Entry(rentable).Collection(r => r.Copies).Load();
        return rentable.Copies.Count(c => !c.Out);
    }
    
    public List<Copy> GetCopiesOfRentable(Rentable rentable)
    {
        var loadedRentable = _dbContext.Rentables
        .Include(r => r.Copies)
        .Where(r => r.Id == rentable.Id)
        .SingleOrDefault();

        if(loadedRentable is null)
            throw new RentalNotFoundException("Rentable could not be found in db");

        return loadedRentable
            .Copies
            .ToList();
    }
    public Copy GetCopy(int copyId) =>
         _dbContext.Copies.Find(copyId)
            ?? throw new VideoArgumentException("Cant find copy!");
    // Returns one available copy of a Rentable
    public Copy GetAvailableCopy(Rentable rentable)
    {
        var availableCopy = GetCopiesOfRentable(rentable)
            .Where(c => !c.Out).FirstOrDefault();

        if(availableCopy is null)
            throw new CopyNotAvailableException("There is no available copies for this rentable");
        
        return availableCopy;
    }
    public Rentable GetRentableOfCopy(int copyId) 
    {
        var rentable = _dbContext.Rentables.Where(r => r.Copies.Any( c => c.Id == copyId)).SingleOrDefault();
        if (rentable is null)
            throw new RentalNotFoundException("Could not load that rentable!!!!");
        return rentable;
    } 

    public void SetDestroyed(RentedCopy rc)
    {
        GetCopy(rc.CopyId).Unusable = true;
        _rentingService.CreateFine(rc, 500m, "Damage");        
        _dbContext.SaveChanges();
    }

    public void SetDamaged(RentedCopy rc)
    {
        GetCopy(rc.CopyId).Damaged = true;
        _rentingService.CreateFine(rc, 1000m, "Destroyed");        
        _dbContext.SaveChanges();
    }
}