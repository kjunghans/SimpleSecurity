using System;
using System.Data.Entity;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleSecurity.Repositories;
using SimpleSecurity.Migrations;
using System.Web.Security;
using System.Linq;

namespace SimpleSecurity.UnitTest
{
    [TestClass]
    public class UnitTestWebSecurity
    {
        private static void InitDB()
        {
            if (!WebMatrix.WebData.WebSecurity.Initialized)
                WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("SimpleSecurityConnection",
                "UserProfile", "UserId", "UserName", autoCreateTables: true);
            var roles = (WebMatrix.WebData.SimpleRoleProvider)Roles.Provider;
            var membership = (WebMatrix.WebData.SimpleMembershipProvider)Membership.Provider;

            if (!roles.RoleExists("Admin"))
            {
                roles.CreateRole("Admin");
            }
            if (!roles.RoleExists("User"))
            {
                roles.CreateRole("User");
            }
            if (!WebSecurity.FoundUser("test"))
            {
                WebSecurity.CreateUserAndAccount("test", "password", "test@gmail.com");
            }
            if (!roles.GetRolesForUser("test").Contains("Admin"))
            {
                roles.AddUsersToRoles(new[] { "test" }, new[] { "admin" });
            }
            if (!WebSecurity.FoundUser("joe"))
            {
                WebSecurity.CreateUserAndAccount("joe", "password", "joe@gmail.com");
            }
            if (!roles.GetRolesForUser("joe").Contains("User"))
            {
                roles.AddUsersToRoles(new[] { "joe" }, new[] { "User" });
            }

        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            AppDomain.CurrentDomain.SetData(
             "DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""));
            SecurityContext context = new SecurityContext();
            context.Database.Delete();
            Database.SetInitializer<SecurityContext>(new MigrateDatabaseToLatestVersion<SecurityContext, Configuration>());
            context.Database.Initialize(true);
            InitDB();
        }

        
        [TestMethod]
        public void TestMethodGetConfirmationToken()
        {
            string username = "jdoe";
            string token = WebSecurity.CreateUserAndAccount(username, "password", "jdoe@gmail.com", true);
            string actualToken = WebSecurity.GetConfirmationToken(username);
            Assert.AreEqual(token, actualToken, "Tokens do not match. Expected " + token + " but returned " + actualToken);
        }

        [TestMethod]
        public void TestValidateUser()
        {
            string username = "jdoe";
            string password = "password";
            string badPassword = "whatthe";
            string token = WebSecurity.CreateUserAndAccount(username, password, "jdoe@gmail.com", false);
            bool validated = WebSecurity.ValidateUser(username, password);
            Assert.IsTrue(validated, "Failed validation of user.");
            validated = WebSecurity.ValidateUser(username, badPassword);
            Assert.IsFalse(validated, "Validation should have failed for bad password.");

        }

        [TestMethod]
        public void TestUpdateUser()
        {
            string username = "jdoe";
            string password = "password";
            string token = WebSecurity.CreateUserAndAccount(username, password, "jdoe@gmail.com", false);
            bool validated = WebSecurity.ValidateUser(username, password);
            Assert.IsTrue(validated, "Failed validation of user.");
            //Get user for update
            var user = WebSecurity.GetUser(username);
            //Change the user name
            username = "jdoe2";
            user.UserName = username;
            //Update the user information
            WebSecurity.UpdateUser(user);
            //Make sure we can still validate them
            validated = WebSecurity.ValidateUser(username, password);
            Assert.IsTrue(validated, "Failed validation of user after update.");

        }
    }
}
