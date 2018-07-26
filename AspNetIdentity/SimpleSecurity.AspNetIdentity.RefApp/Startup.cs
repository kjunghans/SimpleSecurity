using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SimpleSecurity.AspNetIdentity.RefApp.Startup))]
namespace SimpleSecurity.AspNetIdentity.RefApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
