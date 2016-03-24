using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(site.Startup))]
namespace site
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
