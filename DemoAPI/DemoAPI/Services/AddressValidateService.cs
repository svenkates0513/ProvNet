using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DemoAPI.Services
{
    public class AddressValidateService
    {
       /* private readonly ILogger<AddressValidationService> _logger;
        private readonly IValidationLogic _validationLogic;

        public AddressValidationService(IValidationLogic validationLogic, ILogger<AddressValidateService> logger)
        {
            _validationLogic = validationLogic;
            _logger = logger;
        }

        public async Task RunValidationAsync()
        {
            _logger.LogInformation("Starting address validation process.");

            try
            {
                await _validationLogic.ValidateAsync();
                _logger.LogInformation("Address Validation Process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during address validation: {ex.Message}");
            }
        }*/
    }
}
