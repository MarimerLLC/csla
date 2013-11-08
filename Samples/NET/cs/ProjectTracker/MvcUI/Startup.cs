using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcUI.Startup))]
namespace MvcUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
