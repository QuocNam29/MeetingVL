using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MeetingVL.Startup))]
namespace MeetingVL
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
