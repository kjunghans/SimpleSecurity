using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSecurity.Repositories;
using SimpleSecurity.Entities;
using System.Web.Security;

namespace SimpleSecurity
{
    public static class ResourceService
    {
        public static int AddResource(int id, string name, Operations operations)
        {
            UnitOfWork uow = new UnitOfWork();
            Resource resource = new Resource()
            {
                Id = id,
                Name = name,
                Operations = operations
            };

            uow.ResourceRepository.Insert(resource);
            uow.Save();
            return resource.Id;
        }

        public static void MapOperationToRole(int resourceId, Operations operation, string role)
        {
            UnitOfWork uow = new UnitOfWork();
            OperationsToRoles o2r = new OperationsToRoles()
            {
                ResourceId = resourceId,
                Operations = operation,
                RoleName = role
            };
            uow.OperationsToRolesRepository.Insert(o2r);
            uow.Save();
        }

        public static bool Authorize(string userName, int resourceId, Operations operation)
        {
            bool authorized = false;

            var roleProvider = (WebMatrix.WebData.SimpleRoleProvider)Roles.Provider;
            string[] roles = roleProvider.GetRolesForUser(userName);
            UnitOfWork uow = new UnitOfWork();
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
            return authorized;
        }

        public static bool Authorize(int resourceId, Operations operation, string[] roles)
        {
            bool authorized = false;

            UnitOfWork uow = new UnitOfWork();
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
            return authorized;
        }

        public static string GetRolesAsCsv(int resourceId, Operations operation)
        {
            string rolesCsv = string.Empty;
            UnitOfWork uow = new UnitOfWork();
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
            return rolesCsv;

        }

    }
}
