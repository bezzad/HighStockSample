using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HighStockTest.Startup))]
namespace HighStockTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
