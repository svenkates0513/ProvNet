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
    public class OrganizationFinancialDetailController : ControllerBase
    {
        //Logger Helper
        private readonly Logger _logger = new Logger();

        //API Authentication Service
        private readonly HttpClient _client;
        private readonly TokenService _tokenService;

        //DB Connection
        private readonly string _connectionString = "Server=SQLST19A;Database=ProviderCentral;Integrated Security=True";

        public OrganizationFinancialDetailController(HttpClient client, TokenService tokenService)
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
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.veritystream.cloud/services/verityconnect/api/core/v1/organizationfinancialdetails/all");

            //Set the Authorization header (replace {{JWT-Token}} with your actual token)
            //request.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJVc2VySWQiOjE2MzcyLCJSZXNvdXJjZSI6IlZlcml0eSBDb25uZWN0IiwiZXhwIjoxNzMyMTM3ODg1LjB9.lxCLsS_8ScaRWgtodpadsqDfDI8SzYvQ24LzWB1ZZUs0zTKxLrMRTWatD4hFxeCAlzeUVHYhkYBLsJEN3O2W_-IShcbV_Ld--SGjrJ3RYTsG9a7FGt9kN0SvxCrlse1HHwGTdba5EbcY8r3sJhFzQ4fubkh-cn4LKWJ7CV-Kjf0");
            request.Headers.Add("Authorization", $"Bearer {bearerToken}");

            //Define the content to send in the POST request
            var content = new StringContent("{\"Page\": 1, \"PageSize\": 300}", Encoding.UTF8, "application/json");

            request.Content = content;

            try
            {
                //Log request details
                //await _logger.LogAsync($"Request URI: {request.RequestUri}");
                //await _logger.LogAsync($"Request Headers: {request.Headers}");
                //await _logger.LogAsync($"Request Content: {await request.Content.ReadAsStringAsync()}");

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
                List<OrganizationFinancialDetails> organizationFinancialDetails = JsonConvert.DeserializeObject<List<OrganizationFinancialDetails>>(resultArray.ToString());

                //Insert/Update each demographics record in the database
                foreach (var organizationFinancialDetail in organizationFinancialDetails)
                {
                    await InsertOrUpdateOrganizationFinancialDetails(organizationFinancialDetail);
                }

                return Ok(new { message = "Data fetched and inserted/updated successfuly", data = organizationFinancialDetails });
            }

            catch (HttpRequestException httpRequestException)
            {
                //Handle any errors that occur when calling the API
                await _logger.LogAsync($"Error: {httpRequestException.Message}");
                return BadRequest($"Error calling external API : {httpRequestException.Message}");
            }
        }

        // Method to call Store Procedure
        private async Task InsertOrUpdateOrganizationFinancialDetails(OrganizationFinancialDetails organizationFinancialDetails)
        {
            await _logger.LogAsync("Starting DB operation...");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                await _logger.LogAsync("SQL COnnection Opened");

                using (SqlCommand cmd = new SqlCommand("uspInsertOrUpdateOrganizationFinancialDetail", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //Log the command text and eah parameter
                    await _logger.LogAsync($"Executing stored procedure: {cmd.CommandText}");

                    //Paramters to the store procedure
                    cmd.Parameters.AddWithValue("@Id", (object)organizationFinancialDetails.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dr_Id", (object)organizationFinancialDetails.Dr_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Facility_Code", (object)organizationFinancialDetails.Facility_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LocationOtherAddresses_Id", (object)organizationFinancialDetails.LocationOtherAddresses_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UsePhysicalAddress", (object)organizationFinancialDetails.UsePhysicalAddress ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UseMailingAddress", (object)organizationFinancialDetails.UseMailingAddress ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsLockBox", (object)organizationFinancialDetails.IsLockBox ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cycle_Code", (object)organizationFinancialDetails.Cycle_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BillForServices", (object)organizationFinancialDetails.BillForServices ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeliverBillOffsite", (object)organizationFinancialDetails.DeliverBillOffsite ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsMedicaidAccepted", (object)organizationFinancialDetails.IsMedicaidAccepted ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsMedicareAccepted", (object)organizationFinancialDetails.IsMedicareAccepted ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProvideFreeServices", (object)organizationFinancialDetails.ProvideFreeServices ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FinancialInstitution_Code", (object)organizationFinancialDetails.FinancialInstitution_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountName", (object)organizationFinancialDetails.AccountName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountType", (object)organizationFinancialDetails.AccountType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoutingNumber ", (object)organizationFinancialDetails.RoutingNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountNumber", (object)organizationFinancialDetails.AccountNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VoidedCheck", (object)organizationFinancialDetails.VoidedCheck ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankLetter", (object)organizationFinancialDetails.BankLetter ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClaimsClearinghouse_Id", (object)organizationFinancialDetails.ClaimsClearinghouse_Id ?? DBNull.Value);

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
