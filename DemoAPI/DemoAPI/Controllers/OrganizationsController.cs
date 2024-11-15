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
    public class OrganizationsController : ControllerBase
    {
        //Logger Helper
        private readonly Logger _logger = new Logger();

        //API Authentication Service
        //private readonly HttpClient _client;
        //private readonly TokenService _tokenService;

        //DB Connection
        private readonly string _connectionString = "Server=SQLST19A;Database=ProviderCentral;Integrated Security=True";

        //public DemographicController(HttpClient client, TokenService tokenService)
        //{
        //    _client = client;
        //    _tokenService = tokenService;
        //}

        // Create an action method to handle API calls
        [HttpPost("fetch-data")]
        public async Task<IActionResult> FetchDataFromApi()
        {
            //Set up HttpClient and request
            var client = new HttpClient();

            // Get the dynamic bearer token
            //string bearerToken = await _tokenService.GetBearerTokenAsync();

            // Setup the Demographic API request with retrived bearer token
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.veritystream.cloud/services/verityconnect/api/core/v1/organizations/all");

            //Set the Authorization header (replace {{JWT-Token}} with your actual token)
            request.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJVc2VySWQiOjE2MzcyLCJSZXNvdXJjZSI6IlZlcml0eSBDb25uZWN0IiwiZXhwIjoxNzMxNzc0NjI2LjB9.ZbEJgQAbp5TUfySsBxqO9nVld_-u5Ea6_6dCzwpfBvveJVA21R76wZWrdL1VGDHuk9_zkLW9bf05QqtTUWpLs07cu4ijOhV8Up6L3RUlaXz2gC7ALjlH9ykKEDRhrd3RWjkPeTorR3zTzH7yQV4ZGgo0PwNhq4-XRBFFjG_iNOs");
            //request.Headers.Add("Authorization", $"Bearer {bearerToken}");

            //Define the content to send in the POST request
            var content = new StringContent("{\"Page\": 1, \"PageSize\": 20}", Encoding.UTF8, "application/json");

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
                List<Organizations> organizations = JsonConvert.DeserializeObject<List<Organizations>>(resultArray.ToString());

                //Insert/Update each demographics record in the database
                foreach (var organization in organizations)
                {
                    await InsertOrUpdateOrganization(organization);
                }

                return Ok(new { message = "Data fetched and inserted/updated successfuly", data = organizations });
            }

            catch (HttpRequestException httpRequestException)
            {
                //Handle any errors that occur when calling the API
                await _logger.LogAsync($"Error: {httpRequestException.Message}");
                return BadRequest($"Error calling external API : {httpRequestException.Message}");
            }
        }

        // Method to call Store Procedure
        private async Task InsertOrUpdateOrganization(Organizations organizations)
        {
            await _logger.LogAsync("Starting DB operation...");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                await _logger.LogAsync("SQL COnnection Opened");

                using (SqlCommand cmd = new SqlCommand("uspInsertOrUpdateOrganization", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //Log the command text and eah parameter
                    await _logger.LogAsync($"Executing stored procedure: {cmd.CommandText}");

                    //Paramters to the store procedure
                    cmd.Parameters.AddWithValue("@Id", (object)organizations.id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dr_Id", (object)organizations.dr_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExternalId", (object)organizations.externalId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Location_Id", (object)organizations.location_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", (object)organizations.name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SiteType_Code", (object)organizations.siteType_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status_Code", (object)organizations.status_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DoingBusinessAs", (object)organizations.doingBusinessAs ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OtherName", (object)organizations.otherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DisplayOtherName", (object)organizations.displayOtherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HandicapAccess", (object)organizations.handicapAccess ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PublicTransportation", (object)organizations.publicTransportation ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HideFromHub", (object)organizations.hideFromHub ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LocationAddresses_Id", (object)organizations.locationAddresses_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine1", (object)organizations.addressLine1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine2", (object)organizations.addressLine2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)organizations.city ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@State", (object)organizations.state ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Zip", (object)organizations.zip ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ext1", (object)organizations.ext1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone1", (object)organizations.phone1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone2", (object)organizations.phone2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ext2", (object)organizations.ext2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", (object)organizations.fax ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)organizations.email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country", (object)organizations.country ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaxID", (object)organizations.taxID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GroupNPI", (object)organizations.groupNPI ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GroupPTAN", (object)organizations.groupPTAN ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MedicaidNumber", (object)organizations.medicaidNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MedicareNumber", (object)organizations.medicareNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@County", (object)organizations.county ?? DBNull.Value);

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
