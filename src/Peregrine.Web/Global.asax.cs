using System.Web;
using System.Web.Http;
using Autofac;
using Owin;

[assembly: Microsoft.Owin.OwinStartup(typeof(Peregrine.Web.WebApiApplication), "OwinConfiguration")]

namespace Peregrine.Web
{
	public class WebApiApplication : HttpApplication
	{
		public void OwinConfiguration(IAppBuilder app)
		{
			new AuthConfig().Configure(app);
		}

		protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
		}
	}
}
