using System.Web.Http;

namespace Peregrine.Web
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
		}
	}
}
