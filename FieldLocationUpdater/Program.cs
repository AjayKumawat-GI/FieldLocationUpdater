using farm_monitoring_api.Utility;
using FieldLocationUpdater.DataLayer;
using FieldLocationUpdater.Model;
using FieldLocationUpdater.ModelDto;
using FieldLocationUpdater.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

try
{
    Console.WriteLine("Reading Configuration File...");
    var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // Set the base path to the current directory
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    Console.WriteLine("Successfully Read the configuration file!");

    Console.WriteLine("\nInitiating connection with the database!");
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
    Console.WriteLine("Successfully established the connection with the database");

    List<FieldDetail> fields = new List<FieldDetail>();
    if (context != null)
    {
        fields = context.FieldDetails.ToList();
    }

    string baseUrl = configuration["NominatimSettings:BaseUrl"];
    string endPoint = configuration["NominatimSettings:Endpoint"];

    NominatimLocationService locationService = new NominatimLocationService();

    int totalRecords = fields.Count();
    int processedRecords = 0;

    foreach (var field in fields)
    {
        processedRecords++;

        if (field == null)
            continue;

        if (string.IsNullOrEmpty(field.coordinates))
            continue;

        var coordinates = GeoUtils.ExtractFirstCoordinate(field.coordinates);

        var locationDetails = await locationService.GetLocationDetailsAsync(new NominatimLocationRequestDto
        {
            BaseUrl = baseUrl,
            EndPoint = endPoint,
            Latitude = coordinates.Latitude.ToString(),
            Longitude = coordinates.Longitude.ToString(),
        });

        field.state = locationDetails?.address?.state ?? "";
        field.district = locationDetails?.address?.state_district ?? "";
        field.taluka = locationDetails?.address?.county ?? "";
        field.village = new[] {
            locationDetails?.address?.hamlet,
            locationDetails?.address?.village,
            locationDetails?.address?.city
        }
        .FirstOrDefault(value => !string.IsNullOrEmpty(value)) ?? "";

        context.Update(field);
        await context.SaveChangesAsync();

        // Calculate percentage
        double percentage = (double)processedRecords / totalRecords * 100;

        // Display progress
        Console.WriteLine($"Processing record {processedRecords}/{totalRecords} - {percentage:F2}% completed");
    }
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}
Console.ReadLine();
