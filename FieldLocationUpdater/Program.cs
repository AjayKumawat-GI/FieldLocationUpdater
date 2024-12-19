using farm_monitoring_api.Utility;
using FieldLocationUpdater.DataLayer;
using FieldLocationUpdater.Model;
using FieldLocationUpdater.ModelDto;
using FieldLocationUpdater.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

bool isValidInput = false;
bool isUpdated = false;
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

    string farmerId = "";
    string startDateStr = "";
    string endDateStr = "";

    DateTime? startDateTime = null;
    DateTime? endDateTime = null;

    Console.WriteLine();
    Console.WriteLine("Select an option:");
    Console.WriteLine("1 = Update Field based on Farmer ID");
    Console.WriteLine("2 = Update Field based on Date Range");
    Console.WriteLine("3 = Update All Fields");
    Console.Write("Enter any number: ");

    string choice = Console.ReadLine();
    Console.WriteLine();
    switch (choice)
    {
        case "1":
            {
                Console.Write("Enter Farmer ID: ");
                farmerId = Console.ReadLine();
                isValidInput = true;
                break;
            }
        case "2":
            {
                Console.Write("Enter Start Date (yyyy-MM-dd): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                {
                    Console.Write("Enter End Date (yyyy-MM-dd): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                    {
                        if (startDate <= endDate)
                        {
                            // Logic to update fields within the given date range
                            Console.WriteLine($"Updating fields from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}...");
                            // Simulated process
                            startDateTime = startDate;
                            endDateTime = endDate;
                            isValidInput = true;
                        }
                        else
                        {
                            Console.WriteLine("End Date must be greater than or equal to Start Date.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid End Date. Please enter a valid date in yyyy-MM-dd format.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Start Date. Please enter a valid date in yyyy-MM-dd format.");
                }
                break;
            }
        case "3":
            {
                isValidInput = true;
                break;
            }
        default:
            Console.WriteLine("Invalid choice. Exiting...");
            break;
    }

    List<FieldDetail> fields = new List<FieldDetail>();
    if (isValidInput)
    {
        if (context != null)
        {
            fields = context.FieldDetails
                .Where(x => 
                (string.IsNullOrEmpty(farmerId) || x.Farmer_mobile == farmerId) &&
                ((startDateTime == null || endDateTime == null) || (x.upload_date >= startDateTime && x.upload_date <= endDateTime))
                )
                .ToList();
        }
    }

    if(fields != null || fields.Count > 0)
    {
        string baseUrl = configuration["NominatimSettings:BaseUrl"];
        string endPoint = configuration["NominatimSettings:Endpoint"];

        NominatimLocationService locationService = new NominatimLocationService();

        int totalRecords = fields.Count();
        int processedRecords = 0;

        Console.WriteLine();
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
        isUpdated = true;
    }
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
    isUpdated = false;
}
Console.WriteLine();
if (isValidInput && isUpdated)
{
    Console.WriteLine($"Successfully updated all the fields!");
}
else
{
    Console.WriteLine($"Process failed!");
}
Console.ReadLine();
