using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SimpleSecurity.AspNetIdentity.Entities;
using SimpleSecurity.AspNetIdentity.Repositories;

namespace SimpleSecurity.AspNetIdentity
{
    public class WebSecurity : IDisposable
    {
        public UserManager<ApplicationUser> UserManager { get; private set; }

        public WebSecurity()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new SecurityContext())); 
        }

        public ApplicationUser GetUser(string username)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                return uow.ApplicationUserRepository.Get(u => u.UserName == username).SingleOrDefault();
            }
        }

        public ApplicationUser GetCurrentUser()
        {
            return GetUser(CurrentUserName);
        }

        public void CreateUser(ApplicationUser user)
        {
            ApplicationUser dbUser = GetUser(user.UserName);
            if (dbUser != null)
                throw new Exception("User with that username already exists.");
            using (UnitOfWork uow = new UnitOfWork())
            {
                uow.ApplicationUserRepository.Insert(user);
                uow.Save();
            }
        }

        public  bool FoundUser(string username)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                ApplicationUser user = uow.ApplicationUserRepository.Get(u => u.UserName == username).SingleOrDefault();
                return user != null;
            }
        }

        public  string GetEmail(string username)
        {
            string email = null;
            using (UnitOfWork uow = new UnitOfWork())
            {
                ApplicationUser user = uow.ApplicationUserRepository.Get(u => u.UserName == username).SingleOrDefault();
                if (user != null)
                    email = user.Email;
            }
            return email;
        }

        public void Register()
        {
            SecurityContext context = new SecurityContext();
            context.Database.Initialize(true);
        }

        public  bool ValidateUser(string userName, string password)
        {
            var user = UserManager.Find(userName, password);
            if (user != null && user.IsConfirmed)
                return true;

            return false;
        }

 
        public  bool Login(string userName, string password, bool persistCookie = false)
        {
            var user = UserManager.Find(userName, password);
            if (user != null && user.IsConfirmed)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = persistCookie }, identity);
            }
            else return false;

           return true;
        }

        public async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }



        public  bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            ApplicationUser user = UserManager.FindByName(userName);
            if (user == null)
                return false;
            var result = UserManager.ChangePassword(user.Id, oldPassword, newPassword);
            return result.Succeeded;
        }

        public  bool ConfirmAccount(string accountConfirmationToken)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                ApplicationUser user = uow.ApplicationUserRepository.Get(u => u.ConfirmationToken == accountConfirmationToken).SingleOrDefault();
                if (user != null)
                {
                    user.IsConfirmed = true;
                    uow.ApplicationUserRepository.Update(user);
                    uow.Save();
                    return true;
                }
            }
            return false;
        }

        public string CreateUserAndAccount(string userName, string password, string email, bool requireConfirmationToken = false)
        {

            string token = null;
            if (requireConfirmationToken)
             token = ShortGuid.NewGuid();
            bool isConfirmed = !requireConfirmationToken;

            var user = new ApplicationUser()
            {
                UserName = userName,
                Email = email,
                ConfirmationToken = token,
                IsConfirmed = isConfirmed
            };
            var result = UserManager.Create(user, password);
            if (!result.Succeeded)
            {
                StringBuilder innerMsg = new StringBuilder();
                foreach(var msg in result.Errors)
                    innerMsg.Append(msg);
                MembershipCreateUserException ex = new MembershipCreateUserException(innerMsg.ToString());
                throw ex;
            }

            return token;
        }

        public string GetUserId(string userName)
        {
            string id = string.Empty;
            ApplicationUser user = UserManager.FindByName(userName);
            if (user != null)
                id = user.Id;

            return id;
        }

        private  IAuthenticationManager AuthenticationManager
        {
            get
            {
                return System.Web.HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public void Logout()
        {
            
            AuthenticationManager.SignOut();
        }

        public  bool IsAuthenticated { get { return Thread.CurrentPrincipal.Identity.IsAuthenticated; } }

        public  bool IsConfirmed(string username)
        {
            bool isConfirmed = false;
            using (UnitOfWork uow = new UnitOfWork())
            {
                ApplicationUser user = uow.ApplicationUserRepository.Get(u => u.UserName == username).SingleOrDefault();
                if (user != null)
                    isConfirmed = user.IsConfirmed;
            }
            return isConfirmed;
        }

        public string CurrentUserName { get { return Thread.CurrentPrincipal.Identity.GetUserName(); } }

        public string GetCurrentUserId { get { return Thread.CurrentPrincipal.Identity.GetUserId(); } }

        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                ApplicationUser user = uow.ApplicationUserRepository.Get(u => u.UserName == username).SingleOrDefault();
                if (user == null)
                    return false;
                uow.ApplicationUserRepository.Delete(user.Id);
                uow.Save();
            }
            return true;
        }

        public string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440)
        {
            string token = string.Empty;
            using (UnitOfWork uow = new UnitOfWork())
            {
                ApplicationUser user = uow.ApplicationUserRepository.Get(u => u.UserName == userName).SingleOrDefault();
                if (user == null)
                    return string.Empty;
                token = ShortGuid.NewGuid();
                user.PasswordResetToken = token;
                uow.ApplicationUserRepository.Update(user);
                uow.Save();
            }
            return token;
        }

        private  string GetUserIdFromPasswordToken(string passwordResetToken)
        {
            string id = null;
            using (UnitOfWork uow = new UnitOfWork())
            {
                ApplicationUser user = uow.ApplicationUserRepository.Get(u => u.PasswordResetToken == passwordResetToken).SingleOrDefault();
                if (user != null)
                    id = user.Id;
            }
            return id;
        }

        private void RemovePasswordToken(string userId)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                ApplicationUser user = uow.ApplicationUserRepository.GetByID(userId);
                if (user != null)
                {
                    user.PasswordResetToken = null;
                }
                uow.ApplicationUserRepository.Update(user);
                uow.Save();
            }
        }

        public bool ResetPassword(string passwordResetToken, string newPassword)
        {
            string userId = GetUserIdFromPasswordToken(passwordResetToken);
            if (string.IsNullOrEmpty(userId))
                return false;
            //We have to remove the password before we can add it.
            IdentityResult result = UserManager.RemovePassword(userId);
            if (!result.Succeeded)
                return false;
            //We have to add it because we do not have the old password to change it.
            result = UserManager.AddPassword(userId, newPassword);
            if (!result.Succeeded)
                return false;
            
            //Lets remove the token so it cannot be used again.
            RemovePasswordToken(userId);
            //TODO: Should use a timestamp on the token so the reset will not work after a set time.
            return true;
        }

        public string GetConfirmationToken(string userName)
        {
            string token = null;

            using (UnitOfWork uow = new UnitOfWork())
            {
                token = uow.ApplicationUserRepository.Get(u => u.UserName == userName).Select(x => x.ConfirmationToken).SingleOrDefault();
            }
            return token;
        }

        public void MapUserToRole(string userId, string roleName)
        {
                UserManager.AddToRole(userId, roleName);
            
        }

        public Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo)
        {
            return UserManager.FindAsync(loginInfo);
        }

        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            return AuthenticationManager.GetExternalLoginInfoAsync();
        }

        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string xsrfKey, string expectedValue)
        {
            return AuthenticationManager.GetExternalLoginInfoAsync(xsrfKey, expectedValue);
        }

        public Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            return UserManager.AddLoginAsync(userId, login);
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            return UserManager.CreateAsync(user);
        }

        public IList<UserLoginInfo> GetLogins(string userId)
        {
            return UserManager.GetLogins(userId);
        }

        public Task<string> GetPhoneNumberAsync(string userId)
        {
            return UserManager.GetPhoneNumberAsync(userId);
        }

        public Task<bool> GetTwoFactorEnabledAsync(string userId)
        {
            return UserManager.GetTwoFactorEnabledAsync(userId);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            return UserManager.GetLoginsAsync(userId);
        }

        public Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo login)
        {
            return UserManager.RemoveLoginAsync(userId, login);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return UserManager.FindByIdAsync(userId);
        }

        public ApplicationUser FindById(string userId)
        {
            return UserManager.FindById(userId);
        }

        public Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber)
        {
            return UserManager.GenerateChangePhoneNumberTokenAsync(userId, phoneNumber);
        }

        public IIdentityMessageService SmsService
        {
            get { return UserManager.SmsService; }
        }

        public Task<IdentityResult> SetTwoFactorEnabledAsync(string userId, bool enabled)
        {
            return UserManager.SetTwoFactorEnabledAsync(userId, enabled );
        }

        public Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string token)
        {
            return UserManager.ChangePhoneNumberAsync(userId, phoneNumber, token);
        }
        public Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber)
        {
            return UserManager.SetPhoneNumberAsync(userId, phoneNumber);
        }
        public Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            return UserManager.ChangePasswordAsync(userId, currentPassword, newPassword);
        }
        public Task<IdentityResult> AddPasswordAsync(string userId, string newPassword)
        {
            return UserManager.AddPasswordAsync(userId, newPassword);
        }

        public UserManager<ApplicationUser> ThisUserManager
        {
            get {return UserManager;}
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
        }

    }
}
