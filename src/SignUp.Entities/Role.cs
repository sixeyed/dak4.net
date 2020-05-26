using System.Collections;
using System.Collections.Generic;

namespace SignUp.Entities
{
    public class Role
    {
        public string RoleCode { get; set; }

        public string RoleName { get; set; }

        public static IEnumerable<Role> GetSeedData()
        {
            return new List<Role>
            {
                new Role { RoleCode = "-", RoleName = "--Not telling" },
                new Role { RoleCode = "CO", RoleName =  "Consultant" },
                new Role { RoleCode = "TR", RoleName = "Trainer" },
                new Role { RoleCode = "AC", RoleName = "Architect" },
                new Role { RoleCode = "EN", RoleName = "Engineer" },
                new Role { RoleCode = "OP", RoleName = "IT Ops" }
            };
        }
    }
}
