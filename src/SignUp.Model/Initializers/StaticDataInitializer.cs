using SignUp.Entities;
using System.Data.Entity;

namespace SignUp.Model.Initializers
{
    public class StaticDataInitializer : CreateDatabaseIfNotExists<SignUpContext>
    {
        protected override void Seed(SignUpContext context)
        {
            foreach (var role in Role.GetSeedData())
            {
                context.Roles.Add(role);
            }

            foreach (var country in Country.GetSeedData())
            {
                context.Countries.Add(country);
            }

            context.SaveChanges();
        }
    }
}
