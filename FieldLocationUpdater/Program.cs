using FieldLocationUpdater.DataLayer;
using FieldLocationUpdater.ModelDto;
using FieldLocationUpdater.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                    options.UseSqlServer(configuration.GetConnectionString("Con")))
                //.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)) //For logging all the queries executing in sql server
                .BuildServiceProvider();

    // Resolve DbContext and perform operations
    using var context = serviceProvider.GetRequiredService<CarbonDbContext>();

    // Ensure database is created
    context.Database.EnsureCreated();

    if (context != null)
    {
        var fields = context.FieldDetails.ToList();
    }

    string baseUrl = configuration["NominatimSettings:BaseUrl"];
    string endPoint = configuration["NominatimSettings:Endpoint"];

    NominatimLocationService locationService = new NominatimLocationService();

    locationService.GetLocationDetailsAsync(new NominatimLocationRequestDto
    {
        BaseUrl = baseUrl,
        EndPoint = endPoint,
        Latitude = "23.5542",
        Longitude = "73.7095"
    });
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}
Console.ReadLine();
