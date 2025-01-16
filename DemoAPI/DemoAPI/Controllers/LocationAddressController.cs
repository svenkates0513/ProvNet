using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using DemoAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using DemoAPI.Helpers;
using DemoAPI.Services;

namespace DemoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationAddressController : ControllerBase
    {
        //Logger Helper
        private readonly Logger _logger = new Logger();

        //API Authentication Service (TokenService)
        private readonly HttpClient _client;
        private readonly TokenService _tokenService;

        //AddressValidationService
        private readonly AddressValidateService _addressValidateService;

        //DB Connection
        private readonly string _connectionString = "Server=SQLST19A;Database=ProviderCentral;Integrated Security=True";

        public LocationAddressController(HttpClient client, TokenService tokenService, AddressValidateService addressValidateService)
        {
            _client = client;
            _tokenService = tokenService;
            _addressValidateService = addressValidateService;
        }

        // Create an action method to handle API calls
        [HttpPost("fetch-data")]
        public async Task<IActionResult> FetchDataFromApi()
        {
            //Set up HttpClient and request
            var client = new HttpClient();

            // Get the dynamic bearer token
            string bearerToken = await _tokenService.GetBearerTokenAsync();

            // Setup the Loacation Address API request with retrived bearer token
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.veritystream.cloud/services/verityconnect/api/Core/v1/Lookup/LocationAddresses/All");

            //Set the Authorization header (replace {{JWT-Token}} with your actual token)
            request.Headers.Add("Authorization", $"Bearer {bearerToken}");

            //Define the content to send in the POST request
            var content = new StringContent("{\"Page\": 1, \"PageSize\": 300}", Encoding.UTF8, "application/json");

            request.Content = content;

            try
            {
                //Send the request to the external API
                var response = await client.SendAsync(request);

                //Check if the request was successful
                response.EnsureSuccessStatusCode();

                //Read the response content
                var responseData = await response.Content.ReadAsStringAsync();

                //Log the full response content
                //await _logger.LogAsync($"API Response:");
                //await _logger.LogAsync(responseData);

                //Parse the root JSON object to extract the Result array
                JObject root = JObject.Parse(responseData);

                var resultArray = root["Value"]?["Result"];

                if (resultArray == null)
                {
                    await _logger.LogAsync("No Data found in API response");
                    return BadRequest("No Data found in API response");
                }

                //Deserialize JSON response to Demographic object using Newtonsoft.Json
                List<LocationAddress> locationAddress = JsonConvert.DeserializeObject<List<LocationAddress>>(resultArray.ToString());

                //Insert/Update each demographics record in the database
                foreach (var locationAddresses in locationAddress)
                {
                    await InsertOrUpdateLocationAddress(locationAddresses);
                }

                //Trigger the validation service
               // await _addressValidateService.RunValidationAsync();

                return Ok(new { message = "Data fetched and inserted/updated successfuly", data = locationAddress });
            }

            catch (HttpRequestException httpRequestException)
            {
                //Handle any errors that occur when calling the API
                await _logger.LogAsync($"Error: {httpRequestException.Message}");
                return BadRequest($"Error calling external API : {httpRequestException.Message}");
            }
        }

        // Method to call Store Procedure
        private async Task InsertOrUpdateLocationAddress(LocationAddress locationAddress)
        {
            //Log the process
            await _logger.LogAsync("Starting DB operation...");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                //Open the contention and Log it in Logger
                await conn.OpenAsync();
                await _logger.LogAsync("SQL COnnection Opened");

                using (SqlCommand cmd = new SqlCommand("uspInsertOrUpdateLocationAddress", conn))
                {
                    //Open SP-uspInsertOrUpdateLocationAddress and run with value
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //Log the command text and eah parameter
                    await _logger.LogAsync($"Executing stored procedure: {cmd.CommandText}");

                    //Paramters to the store procedure
                    cmd.Parameters.AddWithValue("@Id", (object)locationAddress.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine1", (object)locationAddress.AddressLine1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine2", (object)locationAddress.AddressLine2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)locationAddress.City ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@State", (object)locationAddress.State ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Zip", (object)locationAddress.Zip ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ext1", (object)locationAddress.Ext1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone1", (object)locationAddress.Phone1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone2", (object)locationAddress.Phone2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ext2", (object)locationAddress.Ext2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", (object)locationAddress.Fax ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)locationAddress.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country", (object)locationAddress.Country ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaxID", (object)locationAddress.TaxID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GroupNPI", (object)locationAddress.GroupNPI ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GroupPTAN", (object)locationAddress.GroupPTAN ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MedicaidNumber", (object)locationAddress.MedicaidNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MedicareNumber", (object)locationAddress.MedicareNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@County", (object)locationAddress.County ?? DBNull.Value);

                    // Log each parameter value
                    //foreach (SqlParameter param in cmd.Parameters)
                    //{
                    //    await _logger.LogAsync($"Parameter {param.ParameterName}: {param.Value}");
                    //}

                    //Execute the command and log the result
                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        await _logger.LogAsync("Stored Procedure execute successfully");
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogAsync($"Error executing stored procedure: {ex.Message}");
                        throw; //Re-throw the exception to handle it as need
                    }
                }
            }
        }
    }
}
