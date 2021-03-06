﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSecurity.AspNetIdentity.Entities;

namespace SimpleSecurity.AspNetIdentity.Repositories
{
    public class ResourceRepository: GenericRepository<Resource>
    {
        public ResourceRepository(SecurityContext context) : base(context) { }
    }
}
