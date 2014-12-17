using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Peregrine.Web.Startup))]

namespace Peregrine.Web
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}
