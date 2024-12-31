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
    public class DemographicController : ControllerBase
    {
        //Logger Helper
        private readonly Logger _logger = new Logger();

        //API Authentication Service
        private readonly HttpClient _client;
        private readonly TokenService _tokenService;

        //DB Connection
        private readonly string _connectionString = "Server=SQLST19A;Database=ProviderCentral;Integrated Security=True";

        public DemographicController(HttpClient client, TokenService tokenService)
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
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.veritystream.cloud/services/verityconnect//api/core/v1/Demographics/All");

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
                List<Demographics> demographics = JsonConvert.DeserializeObject<List<Demographics>>(resultArray.ToString());

                //Insert/Update each demographics record in the database
                foreach (var demographic in demographics)
                {
                    await InsertOrUpdateDemographics(demographic);
                }
                
                return Ok(new { message = "Data fetched and inserted/updated successfuly", data = demographics });
            }

            catch (HttpRequestException httpRequestException)
            {
                //Handle any errors that occur when calling the API
                await _logger.LogAsync($"Error: {httpRequestException.Message}");
                return BadRequest($"Error calling external API : {httpRequestException.Message}");
            }
        }

        // Method to call Store Procedure
        private async Task InsertOrUpdateDemographics(Demographics demographics)
        {
            await _logger.LogAsync("Starting DB operation...");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                await _logger.LogAsync("SQL COnnection Opened");

                using (SqlCommand cmd = new SqlCommand("uspInsertOrUpdateDemographic", conn))
                {                    

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //Log the command text and eah parameter
                    await _logger.LogAsync($"Executing stored procedure: {cmd.CommandText}");

                    //Paramters to the store procedure
                    cmd.Parameters.AddWithValue("@Id", (object)demographics.id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dr_Id", (object)demographics.drId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExternalId", (object)demographics.externalId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstName", (object)demographics.firstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MiddleName", (object)demographics.middleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", (object)demographics.lastName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderSuffix_Code", (object)demographics.providerSuffixCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderSalutation_Code", (object)demographics.providerSalutationCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", (object)demographics.gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Birthdate", (object)demographics.birthDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Deathdate", (object)demographics.deathDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryTitles_Code", (object)demographics.primaryTitlesCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EducationDegree_Code", (object)demographics.educationDegreeCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProviderTypes_Id", (object)demographics.providerTypesId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NPDBFieldofLicensureCodes_Code", (object)demographics.npdbFieldofLicensureCodesCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AMA", (object)demographics.ama ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Medicaid", (object)demographics.medicaid ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Medicare", (object)demographics.medicare ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NPI", (object)demographics.npi ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SSN", (object)demographics.ssn ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@USMLE", (object)demographics.usmle ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CAQHID", (object)demographics.caqhid ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CAQHUsername", (object)demographics.caqhUsername ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CAQHPassword", (object)demographics.caqhPassword ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CAQHProviderType_Code", (object)demographics.caqhProviderTypeCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FNINNumber", (object)demographics.fninNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FNINCountryOfIssue_Code", (object)demographics.fninCountryOfIssueCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressLine1", (object)demographics.primaryHomeAddressLine1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressLine2", (object)demographics.primaryHomeAddressLine2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressCity", (object)demographics.primaryHomeAddressCity ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressState_Code", (object)demographics.primaryHomeAddressState_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressZipcode", (object)demographics.primaryHomeAddressZipcode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressCountry_Id", (object)demographics.primaryHomeAddressCountry_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressPhone1", (object)demographics.primaryHomeAddressPhone1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressPhoneExtension1", (object)demographics.primaryHomeAddressPhoneExtension1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressPhone2", (object)demographics.primaryHomeAddressPhone2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryHomeAddressPhoneExtension2", (object)demographics.primaryHomeAddressPhoneExtension2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CellPhone", (object)demographics.cellPhone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", (object)demographics.fax ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pager", (object)demographics.pager ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)demographics.email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Facebook", (object)demographics.facebook ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Twitter", (object)demographics.twitter ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Homepage", (object)demographics.homepage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DriverLicenseNumber", (object)demographics.driverLicenseNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DriverLicenseExpirationDate", (object)demographics.driverLicenseExpirationDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DriverLicenseStateOfIssue_Code", (object)demographics.driverLicenseStateOfIssue_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EthnicOrigins_Code", (object)demographics.ethnicOrigins_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Citizenship", (object)demographics.citizenship ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Countries_Id", (object)demographics.countries_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BirthState_Code", (object)demographics.birthState_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BirthCity", (object)demographics.birthCity ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VisaTypes_Code", (object)demographics.visaTypes_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VisaSponsor", (object)demographics.visaSponsor ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VisaIssuedDate", (object)demographics.visaIssuedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VisaExpiration", (object)demographics.visaExpiration ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VisaNumber", (object)demographics.visaNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VisaStatus_Code", (object)demographics.visaTypes_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrimaryPracticeState_Code", (object)demographics.primaryPracticeState_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MartialStatus_Code", (object)demographics.martialStatus_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SpouseFullName", (object)demographics.spouseFullName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhysicianSpouseName", (object)demographics.physicianSpouseName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HStreamID", (object)demographics.hStreamID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Document1", (object)demographics.document1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AliasId", (object)demographics.aliasId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsOrganization", (object)demographics.isOrganization ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AppModuleEmail", (object)demographics.appModuleEmail ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AppModulePassword", (object)demographics.appModulePassword ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PreferredMethodOfContact_Code", (object)demographics.preferredMethodOfContact_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccessibleFromInternet", (object)demographics.accessibleFromInternet ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NpiEnumerationDate", (object)demographics.npiEnumerationDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CAQHReattestationDate", (object)demographics.caqhReattestationDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CAQHNextReattestationdate", (object)demographics.caqhNextReattestationdate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedDate", (object)demographics.lastModifiedOn ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AOA", (object)demographics.aoa ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sequence", (object)demographics.sequence ?? DBNull.Value);

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
