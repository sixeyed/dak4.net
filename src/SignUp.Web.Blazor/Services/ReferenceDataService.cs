using Microsoft.Extensions.Logging;
using RestSharp;
using SignUp.Core;
using SignUp.Entities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SignUp.Web.Blazor.Services
{
    public class ReferenceDataService
    {
        private readonly ILogger<ReferenceDataService> _logger;

        public ReferenceDataService(ILogger<ReferenceDataService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            var client = new RestClient(Config.Current["ReferenceDataApi:Url"]);
            var request = new RestRequest("countries");
            var countries = await client.GetTaskAsync<List<Country>>(request);
            stopwatch.Stop();

            _logger.LogInformation("Loaded: {CountryCount} countries, took: {CountryLoadMs}ms", countries.Count, stopwatch.ElapsedMilliseconds);
            return countries;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            var client = new RestClient(Config.Current["ReferenceDataApi:Url"]);
            var request = new RestRequest("roles");
            var roles = await client.GetTaskAsync<List<Role>>(request);
            stopwatch.Stop();

            _logger.LogInformation("Loaded: {RoleCount} countries, took: {RoleLoadMs}ms", roles.Count, stopwatch.ElapsedMilliseconds);
            return roles;
        }
    }
}