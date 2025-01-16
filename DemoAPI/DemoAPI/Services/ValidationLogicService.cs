using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DemoAPI.Helpers;
using DemoAPI.Models;
using System.Data.SqlClient;

namespace DemoAPI.Services
{
    public class ValidationLogicService
    {
        //Address Validation API
        private readonly string _validationAPIUrl = "https://taddressvalidation/";

        //Logger
        private readonly Logger _logger;

        //DB Connection
        private readonly string _connectionString = "Server=SQLST19A;Database=ProviderCentral;Integrated Security=True";

        public ValidationLogicService(Logger logger)
        {
            _logger = logger;
        }

        public async Task ValidationAddressesAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                //Fetch unvalidated records from the DB
                var command = new SqlCommand("SELECT ");
            }
        }
    }
}
