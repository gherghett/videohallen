using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using InputHandler;
using VideoHallen.Services;
using VideoHallen.EntryPoints;
using VideoHallen;
using VideoHallen.Models;

namespace VideoHallen;

public static class DummyData
{
    public static void AddTestingData(ServiceProvider serviceProvider)
    {
        var inventoryService = serviceProvider.GetService<InventoryService>()
            ?? throw new NullReferenceException();

        var horror = inventoryService.AddGenre("Horror");
        var action = inventoryService.AddGenre("Action");
        var comedy = inventoryService.AddGenre("Comedy");
        var sciFi = inventoryService.AddGenre("Science Fiction");
        var drama = inventoryService.AddGenre("Drama");

        var topgun = inventoryService.AddMovie(new Movie
        {
            Title = "Top Gun",
            Genres = [action],
            ReleaseDate = DateOnly.Parse("1986-05-12"),
        }, 2);

        var shining = inventoryService.AddMovie(new Movie
        {
            Title = "The Shining",
            Genres = [horror],
            ReleaseDate = DateOnly.Parse("1980-05-23"),
        }, 1);

        var blade = inventoryService.AddMovie(new Movie
        {
            Title = "Blade Runner",
            Genres = [sciFi, action],
            ReleaseDate = DateOnly.Parse("1982-06-25"),
        }, 3);

        var godfather = inventoryService.AddMovie(new Movie
        {
            Title = "The Godfather",
            Genres = [drama, action],
            ReleaseDate = DateOnly.Parse("1972-03-24"),
        }, 2);

        inventoryService.AddMovie(new Movie
        {
            Title = "Ghostbusters",
            Genres = [comedy, action],
            ReleaseDate = DateOnly.Parse("1984-06-08"),
        }, 4);

        var nintendo = inventoryService.AddPublisher("Nintendo");
        var sega = inventoryService.AddPublisher("Sega");
        var atari = inventoryService.AddPublisher("Atari");

        var mario = inventoryService.AddGame(new Game
        {
            Title = "Super Mario Bros.",
            ReleaseDate = DateOnly.Parse("1985-09-13"),
            Publisher = nintendo
        }, 3);

        inventoryService.AddGame(new Game
        {
            Title = "The Legend of Zelda",
            ReleaseDate = DateOnly.Parse("1986-02-21"),
            Publisher = nintendo
        }, 2);

        inventoryService.AddGame(new Game
        {
            Title = "Sonic the Hedgehog",
            ReleaseDate = DateOnly.Parse("1988-12-23"),
            Publisher = sega
        }, 1);

        inventoryService.AddGame(new Game
        {
            Title = "Pac-Man",
            ReleaseDate = DateOnly.Parse("1980-05-22"),
            Publisher = atari
        }, 4);

        inventoryService.AddGame(new Game
        {
            Title = "Metroid",
            ReleaseDate = DateOnly.Parse("1986-08-06"),
            Publisher = nintendo
        }, 2);

        var nes = inventoryService.AddRentConsole("NES", 1);
        inventoryService.AddRentConsole("SEGA Genesis", 5);
        inventoryService.AddRentConsole("Atari", 1);

        var customerService = serviceProvider.GetService<CustomerService>()
            ?? throw new NullReferenceException();

        var maue = customerService.AddCustomer("Mauricio Gherghetta", "042-789 32");
        var micke = customerService.AddCustomer("Mikael Steinholtz", "042-909 18");
        var eva = customerService.AddCustomer("Eva Andersson", "042-123 45");
        var johan = customerService.AddCustomer("Johan Svensson", "042-567 89");
        var lisa = customerService.AddCustomer("Lisa Nilsson", "042-234 56");

        var rentalService = serviceProvider.GetService<RentingService>()
            ?? throw new NullReferenceException();

        rentalService.CreateRental(maue.Id, [
            inventoryService.GetAvailableCopy(topgun).Id,
            inventoryService.GetAvailableCopy(blade).Id
        ], [3, 3]);

        rentalService.CreateRental(micke.Id, [inventoryService.GetAvailableCopy(topgun).Id], [2]);

        rentalService.CreateRental(johan.Id, [
            inventoryService.GetAvailableCopy(shining).Id,
            inventoryService.GetAvailableCopy(mario).Id,
            inventoryService.GetAvailableCopy(nes).Id
        ], [3, 3, 7]);

    }
}
