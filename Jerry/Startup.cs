using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Jerry.Startup))]
namespace Jerry
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
