using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApplication.Startup))]
namespace WebApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
