using Microsoft.EntityFrameworkCore;
namespace VideoHallen.Exceptions;

public abstract class VideoException : Exception
{
    protected VideoException(string message) : base(message)
    {
    }

    protected VideoException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

public class VideoArgumentException : VideoException
{
    public VideoArgumentException(string message) : base(message)
    {
    }

    public VideoArgumentException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

public class CustomerNotFoundException : VideoException
{
    public CustomerNotFoundException(string message) : base(message)
    {
    }

    public CustomerNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

public class CopyNotFoundException : VideoException
{
    public List<int> MissingCopyIds { get; }

    public CopyNotFoundException(string message) : base(message)
    {
        MissingCopyIds = new List<int>();
    }

    public CopyNotFoundException(string message, List<int> missingIds) : base(message)
    {
        MissingCopyIds = missingIds ?? new List<int>();
    }

    public CopyNotFoundException(string message, List<int> missingIds, Exception innerException) 
        : base(message, innerException)
    {
        MissingCopyIds = missingIds ?? new List<int>();
    }
}

public class CopyNotAvailableException : VideoException
{
    public List<int> CopyIds { get; }

    public CopyNotAvailableException(string message) : base(message)
    {
        CopyIds = new List<int>();
    }

    public CopyNotAvailableException(string message, List<int> missingIds) 
        : base(message)
    {
        CopyIds = missingIds ?? new List<int>();
    }

    public CopyNotAvailableException(string message, List<int> missingIds, Exception innerException) 
        : base(message, innerException)
    {
        CopyIds = missingIds ?? new List<int>();
    }
}

public class CustomerHasOutstandingFeesException : VideoException
{
    public CustomerHasOutstandingFeesException(string message) : base(message)
    {
    }

    public CustomerHasOutstandingFeesException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public decimal OutstandingFees { get; }

    public CustomerHasOutstandingFeesException(string message, decimal outstandingFees) 
        : base(message)
    {
        OutstandingFees = outstandingFees;
    }
}

public class RentalNotFoundException : VideoException
{
        public RentalNotFoundException(string message) : base(message)
    {
    }

    public RentalNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public int RentalId { get; }

    public RentalNotFoundException(string message, int id) 
        : base(message)
    {
        RentalId = id;
    }
}
public class RuleBreakException : VideoException
{
    public RuleBreakException(string message) : base(message)
    {
    }

    public RuleBreakException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

public static class ErrorHandler
{
    public static void HandleException(Exception ex)
    {
        switch (ex)
        {
            // case VideoArgumentException argEx:
            //     Console.WriteLine($"Invalid argument: {argEx.Message}");
            //     break;
            // case CustomerNotFoundException cusNFEx:
            //     Console.WriteLine($"Invalid operation: {cusNFEx.Message}");
            //     break;
            // case RentalNotFoundException rentalNFEx:
            //     Console.WriteLine($"Database error: {rentalNFEx.Message}");
            //     break;
            case CopyNotFoundException copyNFEx:
                Console.WriteLine($"{copyNFEx.Message}");
                Console.WriteLine($"Ids not found: {string.Join(", ", copyNFEx.MissingCopyIds)}");
                break;
            case CopyNotAvailableException copyNAvailEx:
                Console.WriteLine($"Database error: {copyNAvailEx.Message}");
                Console.WriteLine($"Ids not found: {string.Join(", ", copyNAvailEx.CopyIds)}");
                break;
            // case CustomerHasOutstandingFeesException custRuleEx:
            //     Console.WriteLine($"Database error: {custRuleEx.Message}");
            //     break;
            case RuleBreakException ruleEx:
                Console.WriteLine(ruleEx.Message);
                break;
            case VideoException vhEx:
                Console.WriteLine(vhEx.Message);
                break;
            // case DbContextException dbcE:
            //     Console.WriteLine("Problem with the database: "+dbcE.Message);
            //     break;
            // case SqliteException sqliteEx:
            //     Console.WriteLine("Problem with the database: "+sqliteEx.Message);
            //     break;
            default: 
                throw ex; //We only want to catch our own exceptions here
        }

    }

}
