using  Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VideoHallen;

/// <summary>
/// This is used by EF Core when generating migrations
/// </summary>
public class VideoHallDbContextFactory : IDesignTimeDbContextFactory<VideoHallDbContext>
{
    public VideoHallDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VideoHallDbContext>();
        optionsBuilder.UseSqlite("Data Source=VideoHall.db");

        return new VideoHallDbContext(optionsBuilder.Options);
    }
}