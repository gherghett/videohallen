using System.Reflection;
using Microsoft.Extensions.Options;
using VideoHallen.Models;
namespace VideoHallen.Services.Pricing;
public interface ITotalPriceStrategy
{

    public decimal CalculateTotalPrice(Rental rental);

}

public class ThreeForTwoMoviesTotalPriceStrategy : ITotalPriceStrategy
{
    private IPricingStrategy _priceStrategy;

    public ThreeForTwoMoviesTotalPriceStrategy(IPricingStrategy pricingStrategy)
    {
        _priceStrategy = pricingStrategy;
    }
    public decimal CalculateTotalPrice(Rental rental)
    {
        decimal totalPrice = 0;
        int movies = 0;
        foreach( var rentedCopy in rental.RentedCopys)
        {
            if(rentedCopy.Copy.Rentable is Movie)
            {
                movies++;
                if(movies % 3 == 0)
                    continue;
            }
            totalPrice += _priceStrategy.CalculatePrice(rentedCopy);
        }
        return totalPrice;
    }
}

public class PercentOffForStammisStrategy : ITotalPriceStrategy
{
    private ITotalPriceStrategy _baseStrategy;
    private decimal _percentOff;

    public PercentOffForStammisStrategy(ITotalPriceStrategy baseStrategy, decimal percentOff)
    {
        _baseStrategy = baseStrategy;
        _percentOff = percentOff;
    }
    public decimal CalculateTotalPrice(Rental rental)
    {
        decimal price = _baseStrategy.CalculateTotalPrice(rental);
        if(IsMoreThanAYearAgo(rental.Customer.JoinDate))
            price *= (100m - _percentOff) * 0.01m;
        return price;
    }
    private static bool IsMoreThanAYearAgo(DateOnly date) => 
        date < DateOnly.FromDateTime(DateTime.Now.AddYears(-1));
}