using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SignUp.Entities;
using SignUp.Web.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SignUp.Web.Core.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<SignUpModel> logger;

        private readonly ReferenceDataService referenceDataService;
        private readonly ProspectSaveService prospectSaveService;

        private static IEnumerable<Country> _Countries;
        private static IEnumerable<Role> _Roles;
        private static readonly Random _Random = new Random();

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Roles { get; set; }

        [BindProperty]
        public Prospect Prospect { get; set; }

        public SignUpModel(ReferenceDataService referenceDataService, ProspectSaveService prospectSaveService, IConfiguration config,  ILogger<SignUpModel> logger)
        {
            this.configuration = config;
            this.logger = logger;
            this.prospectSaveService = prospectSaveService;
            this.referenceDataService = referenceDataService;
        } 

        public async Task<IActionResult> OnGetAsync()
        {
            if (TakedownModel.IsOffline)
            {
                return new StatusCodeResult(503);
            }

            if (_Roles == null)
            {
                _Roles = await referenceDataService.GetRolesAsync();
            }
            if (_Countries == null)
            {
                _Countries = await referenceDataService.GetCountriesAsync();
            }

            Roles = _Roles.Select(x=> new SelectListItem
            {
                Value = x.RoleCode,
                Text = x.RoleName
            }).ToList();

            Countries = _Countries.Select(x => new SelectListItem
            {
                Value = x.CountryCode,
                Text = x.CountryName
            }).ToList();

            Prospect = GetProspect();

            return Page();
        }

        public IActionResult OnPost()
        {
            Prospect.Country = _Countries.Single(x => x.CountryCode == Prospect.Country.CountryCode);
            Prospect.Role = _Roles.Single(x => x.RoleCode == Prospect.Role.RoleCode);

            prospectSaveService.SaveProspect(Prospect);
            return RedirectToPage("/thankyou");
        }
        
        private Prospect GetProspect()
        {
            var guid = Guid.NewGuid().ToString();
            return configuration.GetValue<bool>("PopulateRandomData") ?
                new Prospect
                {
                    FirstName = guid.Substring(0, 2),
                    LastName = guid.Substring(2, 2),
                    CompanyName = guid.Substring(4, 4),
                    EmailAddress = $"{guid.Substring(0, 2)}@{guid.Substring(2, 2)}.com",
                    Country = _Countries.ElementAt(_Random.Next(1, _Countries.Count() - 1)),
                    Role = _Roles.ElementAt(_Random.Next(1, _Roles.Count() - 1))
                } :
                new Prospect
                {
                    Country = new Country(),
                    Role = new Role()
                };
        }
    }
}