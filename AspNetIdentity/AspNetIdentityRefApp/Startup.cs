using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestMvc5Internet.Startup))]
namespace TestMvc5Internet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
