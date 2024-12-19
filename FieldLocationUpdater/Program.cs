using FieldLocationUpdater.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

try
{
    var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // Set the base path to the current directory
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

    // Read values from appsettings.json
    var appName = configuration["AppSettings:ApplicationName"];
    var version = configuration["AppSettings:Version"];
    var connectionString = configuration.GetConnectionString("Con");

    var serviceProvider = new ServiceCollection()
                .AddDbContext<CarbonDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("Con"))
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information))
                .BuildServiceProvider();

    // Resolve DbContext and perform operations
    using var context = serviceProvider.GetRequiredService<CarbonDbContext>();

    // Ensure database is created
    context.Database.EnsureCreated();

    if (context != null)
    {
        var fields = context.FieldDetails.ToList();
    }

}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}
Console.ReadLine();