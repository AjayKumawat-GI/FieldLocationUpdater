using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace farm_monitoring_api.Utility
{
    public class ApiClient
    {
        public async Task<string> CallApiAsync(string baseUrl, HttpMethod httpMethod, string endpoint, IDictionary<string, string> headers = null, AuthenticationHeaderValue authHeader = null, object requestBody = null, IDictionary<string, string> parameters = null)
        {
            string responseBody = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Set the base address of the API
                    client.BaseAddress = new Uri(baseUrl);

                    if (headers != null)
                    {
                        foreach (var h in headers)
                        {
                            client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        }
                    }

                    if (authHeader != null)
                    {
                        // Add the authorization header if an auth token is provided
                        client.DefaultRequestHeaders.Authorization = authHeader;
                    }

                    // Add the query parameters to the request URL
                    endpoint += GetQueryParams(parameters);

                    // Create the HTTP request
                    HttpRequestMessage request = new HttpRequestMessage(httpMethod, endpoint);

                    // Add the request body if provided
                    if (requestBody != null)
                    {
                        request.Content = ConvertToJson(requestBody);
                        //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }

                    //Setting User Agent
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Carbon", "1.0"));

                    // Send the request and get the response
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Deserialize the response body into the specified type
                    responseBody = await response.Content.ReadAsStringAsync();

                    // Return the deserialized response data
                    return responseBody;
                }
            }
            catch (Exception ex)
            {

            }
            return responseBody;
        }

        public async Task<string> CallApiAsync(string baseUrl, string endpoint, HttpMethod httpMethod, AuthenticationHeaderValue authHeader = null, object requestBody = null, IDictionary<string, string> parameters = null, bool returnJson = true)
        {
            string responseBody = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Set the base address of the API
                    client.BaseAddress = new Uri(baseUrl);

                    if (authHeader != null)
                    {
                        // Add the authorization header if an auth token is provided
                        client.DefaultRequestHeaders.Authorization = authHeader;
                    }

                    // Add the query parameters to the request URL
                    endpoint += GetQueryParams(parameters);

                    // Create the HTTP request
                    HttpRequestMessage request = new HttpRequestMessage(httpMethod, endpoint);

                    // Add the request body if provided
                    if (requestBody != null)
                    {
                        request.Content = ConvertToJson(requestBody);
                        //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }

                    //Setting User Agent
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Carbon", "1.0"));

                    // Send the request and get the response
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Deserialize the response body into the specified type
                    responseBody = await response.Content.ReadAsStringAsync();

                    // Return the deserialized response data
                    return responseBody;
                }
            }
            catch (Exception ex)
            {
            }
            return responseBody;
        }

        public async Task<TResponse> CallApiAsync<TResponse>(string baseUrl, string endpoint, HttpMethod httpMethod, AuthenticationHeaderValue authHeader = null, object requestBody = null, IDictionary<string, string> parameters = null)
        {
            TResponse responseData = default(TResponse);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Set the base address of the API
                    client.BaseAddress = new Uri(baseUrl);

                    if (authHeader != null)
                    {
                        // Add the authorization header if an auth token is provided
                        client.DefaultRequestHeaders.Authorization = authHeader;
                    }

                    // Add the query parameters to the request URL
                    endpoint += GetQueryParams(parameters);

                    // Create the HTTP request
                    HttpRequestMessage request = new HttpRequestMessage(httpMethod, endpoint);

                    // Add the request body if provided
                    if (requestBody != null)
                    {
                        request.Content = ConvertToJson(requestBody);
                        //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }

                    //Setting User Agent
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Carbon", "1.0"));

                    // Send the request and get the response
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Deserialize the response body into the specified type
                    string responseBody = await response.Content.ReadAsStringAsync();
                    responseData = JsonConvert.DeserializeObject<TResponse>(responseBody);

                    // Return the deserialized response data
                    return responseData;
                }
            }
            catch (Exception ex)
            {

            }
            return responseData;
        }

        private StringContent ConvertToJson(dynamic request)
        {
            return new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        }


        private string GetQueryParams(IDictionary<string, string> parameters)
        {
            var queryParams = "";
            if (parameters != null && parameters.Any())
            {
                queryParams = "?" + string.Join("&", parameters.Select(x => x.Key + "=" + HttpUtility.UrlEncode(x.Value)));
            }
            return queryParams;
        }


    }
}
