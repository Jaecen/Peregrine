using System.Web.Http;

namespace Peregrine.Api
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
		}
	}
}
