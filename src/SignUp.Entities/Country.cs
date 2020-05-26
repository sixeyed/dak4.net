using System.Collections.Generic;

namespace SignUp.Entities
{
    public class Country
    {
        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public static IEnumerable<Country> GetSeedData()
        {
            return new List<Country>
            {
                new Country { CountryCode = "-", CountryName = "--Not telling" },
                new Country { CountryCode = "GBR", CountryName = "United Kingdom"},
                new Country { CountryCode = "USA", CountryName = "United States"},
                new Country { CountryCode = "PT", CountryName = "Portugal"},
                new Country { CountryCode = "NOR", CountryName = "Norway"},
                new Country { CountryCode = "SWE", CountryName = "Sweden"},
                new Country { CountryCode = "IRE", CountryName = "Ireland"},
                new Country { CountryCode = "DMK", CountryName = "Denmark"},
                new Country { CountryCode = "IND", CountryName = "India"},
                new Country { CountryCode = "LIT", CountryName = "Lithuania"},
                new Country { CountryCode = "SPN", CountryName = "Spain"},
                new Country { CountryCode = "BGM", CountryName = "Belgium"},
                new Country { CountryCode = "TNL", CountryName = "The Netherlands"},
                new Country { CountryCode = "POL", CountryName = "Poland"}
            };
        }
    }
}
