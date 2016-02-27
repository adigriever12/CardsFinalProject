using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyCards.Startup))]
namespace MyCards
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
