using farm_monitoring_api.Utility;
using FieldLocationUpdater.ModelDto;

namespace FieldLocationUpdater.Service
{
    public class NominatimLocationService
    {
        public async Task<NominatimLocationDetailsResponseDto> GetLocationDetailsAsync(NominatimLocationRequestDto requestDto)
        {
            var response = new NominatimLocationDetailsResponseDto();
            try
            {
                if (string.IsNullOrWhiteSpace(requestDto.BaseUrl))
                {
                    Console.WriteLine("\nBase Url cannot be Empty: " + requestDto.BaseUrl);
                    throw new ArgumentNullException(nameof(requestDto.BaseUrl), "\nBase Url cannot be Empty: ");
                }

                if (string.IsNullOrWhiteSpace(requestDto.EndPoint))
                {
                    Console.WriteLine("\nEndpoint cannot be Empty: " + requestDto.EndPoint);
                    throw new ArgumentNullException(nameof(requestDto.EndPoint), "\nEndpoint cannot be Empty: ");
                }

                if (string.IsNullOrWhiteSpace(requestDto.Latitude))
                {
                    Console.WriteLine("\nLatitude cannot be Empty: " + requestDto.Latitude);
                    throw new ArgumentNullException(nameof(requestDto.Latitude), "\nLatitude cannot be Empty: ");
                }

                if (string.IsNullOrWhiteSpace(requestDto.Longitude))
                {
                    Console.WriteLine("\nLongitude cannot be Empty: " + requestDto.Longitude);
                    throw new ArgumentNullException(nameof(requestDto.Longitude), "\nLongitude cannot be Empty: ");
                }

                requestDto.EndPoint = requestDto.EndPoint
                        .Replace("#lat#", requestDto.Latitude)
                        .Replace("#long#", requestDto.Longitude);

                ApiClient apiClient = new ApiClient();
                response = await apiClient.CallApiAsync<NominatimLocationDetailsResponseDto>(requestDto.BaseUrl, requestDto.EndPoint, HttpMethod.Get, null, null, null);

                if(response == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Process failed!, Unable to fetch location from third party API.");
                    throw new NullReferenceException();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            return response;
        }
    }
}
