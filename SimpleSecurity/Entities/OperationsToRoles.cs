using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSecurity.Entities
{
    public class OperationsToRoles
    {
        public string RoleName { get; set; }
        public int ResourceId { get; set; }
        public Operations Operations { get; set; }

    }
}
