using Microsoft.EntityFrameworkCore;
using VideoHallen;
using VideoHallen.Models;

namespace VideoHallen.Services;
public class InventoryService 
{
    private readonly VideoHallDbContext _dbContext;
    public InventoryService(VideoHallDbContext context)
    {
        _dbContext = context;
    }

    // Movie methods
    public Movie AddMovie(string title, DateOnly release, List<MovieGenre> genres)
    {
        Movie newMovie = new Movie
        {
            Title = title,
            ReleaseDate = release,
            Genres = genres
        };
        return AddMovie(newMovie);
    }

    public Movie AddMovie(Movie movie, int copies = 1)
    {
        _dbContext.Add(movie);
        _dbContext.SaveChanges();
        AddCopies(movie, copies);
        return movie;
    }

    public Movie? GetMovie(int id)
    {
        return _dbContext.Movies.Include(m => m.Genres).FirstOrDefault(m => m.Id == id);
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

    public Game? GetGame(int id)
    {
        return _dbContext.Games.Include(g => g.Publisher).FirstOrDefault(g => g.Id == id);
    }

    // RentConsole methods
    public RentConsole AddRentConsole(string model)
    {
        RentConsole newConsole = new RentConsole { Model = model };
        _dbContext.Add(newConsole);
        _dbContext.SaveChanges();
        return newConsole;
    }

    public RentConsole? GetRentConsole(int id)
    {
        return _dbContext.RentConsoles.Find(id);
    }

    // Copy methods
    public List<Copy> GetAllCopies()
    {
        return _dbContext.Copies.Include(c => c.Rentable).ToList();
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

    // MovieGenre methods
    public List<MovieGenre> GetAllMovieGenres()
    {
        return _dbContext.MovieGenres.ToList();
    }


    internal List<GamePublisher> GetAllGamePublishers()
    {
        return _dbContext.GamePublishers.ToList();
    }
    public List<Rentable> GetAllInventory()
    {
        return _dbContext.Rentables.Include(r => r.Copies).ToList();
    }

    internal List<Movie> GetAllMovies()
    {
        return _dbContext.Movies.Include(r => r.Copies).ToList();
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

    public MovieGenre AddGenre(string name)
    {
        var genre = new MovieGenre{
            Name = name,
        };
        _dbContext.Add(genre);
        _dbContext.SaveChanges();
        return genre;
    }
}