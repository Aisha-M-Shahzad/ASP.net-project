using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CarFinalProject.Startup))]
namespace CarFinalProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
