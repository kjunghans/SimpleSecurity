using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSecurity.AspNetIdentity.Repositories;
using SimpleSecurity.AspNetIdentity.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SimpleSecurity.AspNetIdentity
{
    public static class ResourceService
    {
        public static int AddResource(int id, string name, Operations operations)
        {

            Resource resource = new Resource()
            {
                Id = id,
                Name = name,
                Operations = operations
            };

            using (UnitOfWork uow = new UnitOfWork())
            {
                uow.ResourceRepository.Insert(resource);
                uow.Save();
                return resource.Id;
            }
        }

        public static bool FoundResource(int id)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                Resource resource = uow.ResourceRepository.GetByID(id);
                return resource != null;
            }
        }

        public static void MapOperationToRole(int resourceId, Operations operation, string role)
        {
            OperationsToRoles o2r = new OperationsToRoles()
            {
                ResourceId = resourceId,
                Operations = operation,
                RoleName = role
            };
            using (UnitOfWork uow = new UnitOfWork())
            {
                uow.OperationsToRolesRepository.Insert(o2r);
                uow.Save();
            }
        }

        public static bool Authorize(string userName, int resourceId, Operations operation)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new SecurityContext()));
            ApplicationUser user = UserManager.FindByName(userName);
            string[] roles = UserManager.GetRoles(user.Id).ToArray();
            return Authorize(resourceId, operation, roles);
        }

        public static bool Authorize(int resourceId, Operations operation, string[] roles)
        {
            bool authorized = false;

            using (UnitOfWork uow = new UnitOfWork())
            {
                foreach (string role in roles)
                {
                    int count = uow.OperationsToRolesRepository.Get(o => o.RoleName == role && o.ResourceId == resourceId &&
                         (o.Operations & operation) != 0).Count();
                    if (count > 0)
                    {
                        authorized = true;
                        break;
                    }
                }
            }
            return authorized;
        }

        public static string GetRolesAsCsv(int resourceId, Operations operation)
        {
            string rolesCsv = string.Empty;
            using (UnitOfWork uow = new UnitOfWork())
            {
                var roles = uow.OperationsToRolesRepository.Get(o => o.ResourceId == resourceId &&
                    (o.Operations & operation) != 0).Select(o => o.RoleName).Distinct();
                bool firstItem = true;
                foreach (string role in roles)
                {
                    if (firstItem)
                    {
                        rolesCsv += role;
                        firstItem = false;
                    }
                    else
                    {
                        rolesCsv += "," + role;
                    }
                }
            }
            return rolesCsv;

        }

    }
}
