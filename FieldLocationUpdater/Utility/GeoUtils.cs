using FieldLocationUpdater.ModelDto;

namespace farm_monitoring_api.Utility
{
    public class GeoUtils
    {
        public static LatLongDto ExtractFirstCoordinate(string multipolygon)
        {
            // Check if the input string is null or empty
            if (string.IsNullOrWhiteSpace(multipolygon))
            {
                return new LatLongDto
                {
                    Latitude = 0,
                    Longitude = 0
                };
            }

            try
            {
                // Remove the wrapper
                string coordinates = multipolygon
                    .Replace("MULTIPOLYGON (((", "", StringComparison.OrdinalIgnoreCase)
                    .Replace(")))", "", StringComparison.OrdinalIgnoreCase);

                // Split into individual coordinate groups
                string[] coordinatePairs = coordinates.Split(',');

                if (coordinatePairs.Length == 0)
                {
                    return new LatLongDto
                    {
                        Latitude = 0,
                        Longitude = 0
                    };
                }

                // Extract the first coordinate pair
                string[] firstCoordinate = coordinatePairs[0].Trim().Split(' ');

                if (firstCoordinate.Length < 2)
                {
                    return new LatLongDto
                    {
                        Latitude = 0,
                        Longitude = 0
                    };
                }

                // Parse longitude and latitude
                decimal longitude = decimal.Parse(firstCoordinate[0]);
                decimal latitude = decimal.Parse(firstCoordinate[1]);

                return new LatLongDto
                {
                    Latitude = latitude,
                    Longitude = longitude
                };
            }
            catch
            {
                // Return empty latitude and longitude in case of any parsing issues
                return new LatLongDto
                {
                    Latitude = 0,
                    Longitude = 0
                };
            }
        }

    }
}
