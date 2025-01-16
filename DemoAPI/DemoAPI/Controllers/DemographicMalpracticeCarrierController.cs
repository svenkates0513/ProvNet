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
    public class DemographicMalpraticeCarrierController : ControllerBase
    {
        //Logger Helper
        private readonly Logger _logger = new Logger();

        //API Authentication Service
        private readonly HttpClient _client;
        private readonly TokenService _tokenService;

        //DB Connection
        private readonly string _connectionString = "Server=SQLST19A;Database=ProviderCentral;Integrated Security=True";

        public DemographicMalpraticeCarrierController(HttpClient client, TokenService tokenService)
        {
            _client = client;
            _tokenService = tokenService;
        }

        // Create an action method to handle API calls
        [HttpPost("fetch-data")]
        public async Task<IActionResult> FetchDataFromApi()
        {
            //Set up HttpClient and request
            var client = new HttpClient();

            // Get the dynamic bearer token
            string bearerToken = await _tokenService.GetBearerTokenAsync();

            // Setup the Demographic API request with retrived bearer token
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.veritystream.cloud/services/verityconnect/api/core/v1/Demographic/MalpracticeCarriers/all");

            //Set the Authorization header (replace {{JWT-Token}} with your actual token)
            //request.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJVc2VySWQiOjE2MzcyLCJSZXNvdXJjZSI6IlZlcml0eSBDb25uZWN0IiwiZXhwIjoxNzMyMTM3ODg1LjB9.lxCLsS_8ScaRWgtodpadsqDfDI8SzYvQ24LzWB1ZZUs0zTKxLrMRTWatD4hFxeCAlzeUVHYhkYBLsJEN3O2W_-IShcbV_Ld--SGjrJ3RYTsG9a7FGt9kN0SvxCrlse1HHwGTdba5EbcY8r3sJhFzQ4fubkh-cn4LKWJ7CV-Kjf0");
            request.Headers.Add("Authorization", $"Bearer {bearerToken}");

            //Define the content to send in the POST request
            var content = new StringContent("{\"Page\": 1, \"PageSize\": 300}", Encoding.UTF8, "application/json");

            request.Content = content;

            try
            {
                //Send the request to the external API
                //var response = await _client.SendAsync(request);
                var response = await client.SendAsync(request);

                //Check if the request was successful
                response.EnsureSuccessStatusCode();

                //Read the response content
                var responseData = await response.Content.ReadAsStringAsync();

                //Log the full response content
                await _logger.LogAsync($"API Response:");
                await _logger.LogAsync(responseData);

                //Parse the root JSON object to extract the Result array
                JObject root = JObject.Parse(responseData);

                var resultArray = root["Value"]?["Result"];

                if (resultArray == null)
                {
                    await _logger.LogAsync("No Data found in API response");
                    return BadRequest("No Data found in API response");
                }

                //Deserialize JSON response to Demographic object using Newtonsoft.Json
                List<DemographicMalpracticeCarriers> demographicMalpracticeCarriers = JsonConvert.DeserializeObject<List<DemographicMalpracticeCarriers>>(resultArray.ToString());

                //Insert/Update each demographics record in the database
                foreach (var demographicMalpracticeCarrier in demographicMalpracticeCarriers)
                {
                    await InsertOrUpdateDemographicMalpracticeCarriers(demographicMalpracticeCarrier);
                }

                return Ok(new { message = "Data fetched and inserted/updated successfuly", data = demographicMalpracticeCarriers });
            }

            catch (HttpRequestException httpRequestException)
            {
                //Handle any errors that occur when calling the API
                await _logger.LogAsync($"Error: {httpRequestException.Message}");
                return BadRequest($"Error calling external API : {httpRequestException.Message}");
            }
        }

        // Method to call Store Procedure
        private async Task InsertOrUpdateDemographicMalpracticeCarriers(DemographicMalpracticeCarriers demographicMalpracticeCarriers)
        {
            await _logger.LogAsync("Starting DB operation...");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                await _logger.LogAsync("SQL COnnection Opened");

                using (SqlCommand cmd = new SqlCommand("uspInsertOrUpdateDemographicMalpracticeCarrier", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //Log the command text and eah parameter
                    await _logger.LogAsync($"Executing stored procedure: {cmd.CommandText}");

                    //Paramters to the store procedure
                    cmd.Parameters.AddWithValue("@Id", (object)demographicMalpracticeCarriers.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dr_Id", (object)demographicMalpracticeCarriers.Dr_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Type_Code", (object)demographicMalpracticeCarriers.Type_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Carrier_Code", (object)demographicMalpracticeCarriers.Carrier_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comment1", (object)demographicMalpracticeCarriers.Comment1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comment2", (object)demographicMalpracticeCarriers.Comment2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PolicyNumber", (object)demographicMalpracticeCarriers.PolicyNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tail", (object)demographicMalpracticeCarriers.Tail ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Expires", (object)demographicMalpracticeCarriers.Expires ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TicklerDate", (object)demographicMalpracticeCarriers.TicklerDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TicklerStatus", (object)demographicMalpracticeCarriers.TicklerStatus ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Enrolled", (object)demographicMalpracticeCarriers.Enrolled ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Document1", (object)demographicMalpracticeCarriers.Document1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PRIVATE", (object)demographicMalpracticeCarriers.PRIVATE ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Facility_Code", (object)demographicMalpracticeCarriers.Facility_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CoverageAggregate", (object)demographicMalpracticeCarriers.CoverageAggregate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CoveragePerIncident", (object)demographicMalpracticeCarriers.CoverageAggregate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Current_Issue_Date", (object)demographicMalpracticeCarriers.Current_Issue_Date ?? DBNull.Value);

                    // Log each parameter value
                    foreach (SqlParameter param in cmd.Parameters)
                    {
                        await _logger.LogAsync($"Parameter {param.ParameterName}: {param.Value}");
                    }

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
