using Microsoft.EntityFrameworkCore; // To use UseSqlite.

namespace RfidApi.Data;

// ServiceCollection which will provide extension methods for adding services and configure the database
public static class AppDbContextExtensions
{
    public static IServiceCollection AddItemServices(
        this IServiceCollection services,
        string relativePath = "..",
        string dbName = "rfid.db"
    )
    {
        string basePath = AppContext.BaseDirectory;
        string path = Path.Combine(relativePath, dbName);
        path = Path.GetFullPath(path);
        Console.WriteLine($"Database path: {path}");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(message: $"{path} not found.", fileName: path);
        }

        services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={path}"));
        return services;
    }
}
