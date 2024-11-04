using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoAPI.Services
{
    public class TokenService
    {
        private readonly HttpClient _client;

        public TokenService(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetBearerTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.veritystream.cloud/services/oauth/api/authentication/jwt/validate");

            //Adding Header
            //request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Pragma", "no-cache");
           // request.Headers.Add("Expires", "-1");

            //Set the JSON body with the required structure
            var body = new
            {
                Requester = new
                {
                    Key = "65062D3C-ECF8-4743-A8CC-58CF4F2BFC0E",
                    Id = "39307",
                    Secret = "gLQH1LJHDwpPtahq",
                    Resource = "Verity Connect",
                    Instance = ""
                },
                Parameters = new { }
            };

            string jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            request.Content = content;

            //Send the request
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            //Get the response content
            var responseContent = await response.Content.ReadAsStringAsync();

            //Parse the JSON to extract the token
            var json = JsonDocument.Parse(responseContent);
            string token = json.RootElement.GetProperty("Value").GetString();

            return token;
        }
    }
}
