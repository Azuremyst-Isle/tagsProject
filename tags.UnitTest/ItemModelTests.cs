using Microsoft.EntityFrameworkCore;
using RfidApi.Data;
using RfidApi.Models;

namespace tags.UnitTest;

public class ItemModelTests
{
    private string filepath()
    {
        string dir = Environment.CurrentDirectory;
        string path = string.Empty;
        if (dir.EndsWith("net8.0"))
        {
            path = Path.Combine(dir, "..", "..", "..", "..");
        }
        else
        {
            path = Path.Combine(dir, "..");
        }
        path = Path.GetFullPath(path);
        return path;
    }

    private DbContextOptions<AppDbContext> dbOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={filepath()}\\instance\\rfid.db")
            .Options;
    }

    [Fact]
    public void DatabaseConnectTest()
    {
        using AppDbContext db = new(dbOptions());
        Assert.True(db.Database.CanConnect());
    }

    [Fact]
    public void CurrentPathTest()
    {
        string expected = "C:\\Users\\alfa3\\OneDrive\\Webdev\\Francesco\\RFI\\tagsProject";
        string dir = Environment.CurrentDirectory;
        string current = filepath();
        Assert.Equal(expected, current);
    }

    [Fact]
    public void ItemCountTest()
    {
        using AppDbContext db = new(dbOptions());
        // Given
        int expected = 3;
        // When
        int actual = db.item.Count();

        // Then
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindItemByIdTest()
    {
        using AppDbContext db = new(dbOptions());
        // Given
        int itemId = 1;
        // When
        var item = db.item.Find(keyValues: 1);
        // Then
        Assert.NotNull(item);
        Assert.Equal(itemId, item.Id);
    }

    [Fact]
    public void CheckStatusTest()
    {
        // Given
        using AppDbContext db = new(dbOptions());
        int itemId = 1;
        var item = db.item.Find(itemId);
        Assert.NotNull(item);

        // When
        string status = item.status;

        // Then
        Assert.NotNull(status);
        Assert.Equal("available", status);
    }

    [Fact]
    public void CheckNameTest()
    {
        // Given
        using AppDbContext db = new(dbOptions());
        int itemId = 1;
        var item = db.item.Find(keyValues: itemId);
        Assert.NotNull(item);

        // When
        string? name = item.name;

        // Then
        Assert.NotNull(name);
        Assert.Equal("Tony", name);
    }

    [Fact]
    public void InsertItemTest()
    {
        using AppDbContext db = new(dbOptions());

        var newItem = new Item
        {
            rfid_tag = "test123",
            name = "TestName",
            description = "Test Description",
            status = "available",
            certification_code = null,
            owner_name = "UnitTester",
            last_updated = DateTime.UtcNow,
        };

        db.item.Add(newItem);
        db.SaveChanges();

        var inserted = db.item.FirstOrDefault(i => i.rfid_tag == "test123");
        Assert.NotNull(inserted);
        Assert.Equal("TestName", inserted.name);
    }
}
