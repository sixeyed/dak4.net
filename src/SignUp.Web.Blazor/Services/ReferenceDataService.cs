using RestSharp;
using SignUp.Core;
using SignUp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignUp.Web.Blazor.Services
{
    public class ReferenceDataService
    {
        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            return new List<Country> {"A", "B", "C"};
                        /*
            var client = new RestClient(Config.Current["ReferenceDataApi:Url"]);
            var request = new RestRequest("countries");
            return await client.GetTaskAsync<List<Country>>(request);
            */
        }        

        public async Task<IEnumerable<Role>> GetRoles()
        {
            return new List<Role> {"1", "2"};
            /*
            var client = new RestClient(Config.Current["ReferenceDataApi:Url"]);
            var request = new RestRequest("roles");
            return await client.GetTaskAsync<List<Role>>(request);
            */
        }
    }
}