﻿using Microsoft.AspNetCore.Mvc;
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
    public class LocationController : ControllerBase
    {
        //Logger Helper
        private readonly Logger _logger = new Logger();

        //API Authentication Service
        private readonly HttpClient _client;
        private readonly TokenService _tokenService;

        //DB Connection
        private readonly string _connectionString = "Server=SQLST19A;Database=ProviderCentral;Integrated Security=True";

        public LocationController(HttpClient client, TokenService tokenService)
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
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.veritystream.cloud/services/verityconnect/api/core/v1/lookup/locations/all");

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
                List<Locations> locations = JsonConvert.DeserializeObject<List<Locations>>(resultArray.ToString());

                //Insert/Update each demographics record in the database
                foreach (var location in locations)
                {
                    await InsertOrUpdateLocations(location);
                }

                return Ok(new { message = "Data fetched and inserted/updated successfuly", data = locations });
            }

            catch (HttpRequestException httpRequestException)
            {
                //Handle any errors that occur when calling the API
                await _logger.LogAsync($"Error: {httpRequestException.Message}");
                return BadRequest($"Error calling external API : {httpRequestException.Message}");
            }
        }

        // Method to call Store Procedure
        private async Task InsertOrUpdateLocations(Locations locations)
        {
            await _logger.LogAsync("Starting DB operation...");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                await _logger.LogAsync("SQL COnnection Opened");

                using (SqlCommand cmd = new SqlCommand("uspInsertOrUpdateLocations", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //Log the command text and eah parameter
                    await _logger.LogAsync($"Executing stored procedure: {cmd.CommandText}");

                    //Paramters to the store procedure
                    cmd.Parameters.AddWithValue("@Id", (object)locations.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dr_Id", (object)locations.Dr_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", (object)locations.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExternalId", (object)locations.ExternalId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LocationAddresses_Id", (object)locations.LocationAddresses_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SiteType_Code", (object)locations.SiteType_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status_Code", (object)locations.Status_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DoingBusinessAs", (object)locations.DoingBusinessAs ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OtherName", (object)locations.OtherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DisplayOtherName", (object)locations.DisplayOtherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Region", (object)locations.Region ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HandicapAccess", (object)locations.HandicapAccess ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PublicTransportation", (object)locations.PublicTransportation ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HideFromHub", (object)locations.HideFromHub ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DisplayName", (object)locations.DisplayName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Url", (object)locations.Url ?? DBNull.Value);

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
