using Microsoft.EntityFrameworkCore;
using VideoHallen;
using VideoHallen.Models;
using VideoHallen.Exceptions;

namespace VideoHallen.Services;
public class InventoryService 
{
    private readonly VideoHallDbContext _dbContext;
    public InventoryService(VideoHallDbContext context)
    {
        _dbContext = context;
    }

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
    public Game AddGame(string title, DateOnly release, GamePublisher publisher)
    {
        Game newGame = new Game
        {
            Title = title,
            ReleaseDate = release,
            Publisher = publisher
        };
        _dbContext.Add(newGame);
        _dbContext.SaveChanges();
        return newGame;
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
    public RentConsole AddRentConsole(string model)
    {
        RentConsole newConsole = new RentConsole { Model = model };
        _dbContext.Add(newConsole);
        _dbContext.SaveChanges();
        return newConsole;
    }

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

    public List<Rentable> GetAllInventory()
    {
        return _dbContext.Rentables.Include(r => r.Copies).ToList();
    }

    internal List<Game> GetAllGames()
    {
        return _dbContext.Games
            .Include(g => g.Copies)
                .ThenInclude(c => c.Rentals)
                    .ThenInclude(r => r.Returns)
                        .ThenInclude(r => r.ReturnedCopies)
            .ToList();
    }

    internal List<RentConsole> GetAllConsoles()
    {
        return _dbContext.RentConsoles.Include(r => r.Copies).ToList();
    }

    public int GetAvailability(Rentable rentable)
    {
        _dbContext.Entry(rentable).Collection(r => r.Copies).Load();
        return rentable.Copies.Count(c => !c.Out);
    }

    //Search
    public List<Rentable> SearchAllInventory(string query)
    {
        var movies = SearchMovies(query);
        var games = SearchGames(query);
        var consoles = SearchRentConsoles(query);
        
        return movies.Cast<Rentable>()
                 .Concat(games.Cast<Rentable>())
                 .Concat(consoles.Cast<Rentable>())
                 .ToList();
    }
    public List<Movie> SearchMovies(string query)
    {
        return _dbContext.Movies.Where(c => EF.Functions.Like(c.Title, $"%{query}%")).ToList();
    }
    public List<Game> SearchGames(string query)
    {
        return _dbContext.Games.Where(c => EF.Functions.Like(c.Title, $"%{query}%")).ToList();
    }
    public List<RentConsole> SearchRentConsoles(string query)
    {
        return _dbContext.RentConsoles.Where(c => EF.Functions.Like(c.Model, $"%{query}%")).ToList();
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
    public Copy GetAvailableCopy(Rentable rentable)
    {
        var availableCopy = GetCopiesOfRentable(rentable)
            .Where(c => !c.Out).FirstOrDefault();

        if(availableCopy is null)
            throw new CopyNotAvailableException("There is no available copies for this rentable");
        
        return availableCopy;
    }
}