using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Peregrine.Service
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Attribute-based routing
			config.MapHttpAttributeRoutes();

			// Convention-based routing
			//config.Routes.MapHttpRoute(
			//	name: "DefaultApi",
			//	routeTemplate: "api/{controller}/{id}",
			//	defaults: new { id = RouteParameter.Optional }
			//);
		}
	}
}
