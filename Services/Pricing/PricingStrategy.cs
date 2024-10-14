using System.Reflection;
using Microsoft.Extensions.Options;
using VideoHallen.Models;
namespace VideoHallen.Services.Pricing;

public interface IPricingStrategy
{
    decimal CalculatePrice(RentedCopy rentedCopy);
}

// public class SimplePricingOptions
// {
//     public decimal MoviePerDay {get; set;}
//     public decimal GamePerDay {get; set;}
//     public decimal ConsolePerDay {get; set;}
//     public decimal ConsolePerWeek {get; set;}
// }
public class SimplePricingStrategy : IPricingStrategy
{
    private readonly decimal _moviePerDay;
    private readonly decimal _gamePerDay;
    private readonly decimal _consolePerDay;
    private readonly decimal _consolePerWeek;


    // public SimplePricing(IOptions<SimplePricingOptions> options)
    // {
    //     _moviePerDay = options.Value.MoviePerDay;
    //     _gamePerDay = options.Value.GamePerDay;
    //     _consolePerDay = options.Value.ConsolePerDay;
    //     _consolePerWeek = options.Value.ConsolePerWeek;
    // }
    public SimplePricingStrategy(
        decimal moviePerDay,
        decimal gamePerDay,
        decimal consolePerDay,
        decimal consolePerWeek
    )
    {
        _moviePerDay = moviePerDay;
        _gamePerDay = gamePerDay;
        _consolePerDay = consolePerDay;
        _consolePerWeek = consolePerWeek;
    }
    public decimal CalculatePrice(RentedCopy rentedCopy)
    {
        var startOfRent = rentedCopy.Rental.TimeStamp;
        int daysOfRent = (DateTime.Parse(rentedCopy.DueByDate.ToString()) - rentedCopy.Rental.TimeStamp.Date).Days;
        if( rentedCopy.Copy.Rentable is Movie )
            return _moviePerDay * daysOfRent;
        if(rentedCopy.Copy.Rentable is Game )
            return _gamePerDay * daysOfRent;
        if( rentedCopy.Copy.Rentable is RentConsole)
        {
            if ( daysOfRent == 7)
                return _consolePerWeek;
            else
                return _consolePerDay * daysOfRent;
        }
        throw new ArgumentException("Unregognized type of rentabled");
    }

}