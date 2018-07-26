using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSecurity.AspNetIdentity.Entities;

namespace SimpleSecurity.AspNetIdentity.Repositories
{
    public class ApplicationUserRepository: GenericRepository<ApplicationUser>
    {
        public ApplicationUserRepository(SecurityContext context) : base(context) { }
    }
}
