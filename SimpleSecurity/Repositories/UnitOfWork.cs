using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSecurity.Entities;

namespace SimpleSecurity.Repositories
{
    public class UnitOfWork  : IDisposable
    {
        private SecurityContext _context = new SecurityContext();
        private UserProfileRepository _userProfileRepository;
        private MembershipRepository _membershipRepository;
        private ResourceRepository _resourceRepository;
        private OperationsToRolesRepository _operationsToRolesRepository;

        public UnitOfWork()
        {
            _userProfileRepository = new UserProfileRepository(_context);
            _membershipRepository = new MembershipRepository(_context);
            _operationsToRolesRepository = new OperationsToRolesRepository(_context);
            _resourceRepository = new ResourceRepository(_context);
        }

        public UserProfileRepository UserProfileRepository
        {
            get { return _userProfileRepository; }
        }

        public MembershipRepository MembershipRepository
        {
            get { return _membershipRepository; }
        }

        public ResourceRepository ResourceRepository
        {
            get { return _resourceRepository; }
        }

         public OperationsToRolesRepository OperationsToRolesRepository
        {
            get { return _operationsToRolesRepository; }
        }

        public int Save()
         {
            return _context.SaveChanges();
         }

        public SecurityContext Context
        {
            
            get{
                return _context;
            }
        }

       private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
   
}
