using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SignUp.Api.ReferenceData.Controllers
{
    [Produces("application/json")]
    [Route("api/ready")]
    public class ReadyController : Controller
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;

        public ReadyController(IConfiguration configuration, ILogger<ReadyController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("SignUpDb");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                    _logger.LogInformation("Successfully connected to SQL Server");
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception opening SQL Server connection: {message}", ex.Message);
                return StatusCode(500);
            }
        }
    }
}