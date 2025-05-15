using AutoMapper;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Service.Mapping;

namespace Tests;

public abstract class TestBase : IDisposable
{
    protected readonly ApplicationDbContext Context;
    protected readonly IMapper Mapper;
    private readonly string _dbName;

    protected TestBase()
    {
        _dbName = $"TestDb_{GetType().Name}_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;

        Context = new ApplicationDbContext(options);
        
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });
        Mapper = mappingConfig.CreateMapper();
    }

    protected async Task ClearDatabase()
    {
        Context.Products.RemoveRange(Context.Products);
        Context.Categories.RemoveRange(Context.Categories);
        await Context.SaveChangesAsync();
        
        await Context.Database.EnsureDeletedAsync();
        await Context.Database.EnsureCreatedAsync();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
} 