using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using Peregrine.Service.Model;

namespace Peregrine.Service.Services
{
	public class ActionLinkBuilder
	{
		public IEnumerable<ActionLink> BuildActions(HttpControllerContext controllerContext)
		{
			return controllerContext
				.ControllerDescriptor
				.Configuration
				.Services
				.GetApiExplorer()
				.ApiDescriptions
				.Where(ad => ad.ActionDescriptor.ControllerDescriptor == controllerContext.ControllerDescriptor)	// Find actions for the current resource
				.Select(ad => new																					// Pull out the route name from the route attribute
				{
					Api = ad,
					RouteName = ad.ActionDescriptor.GetCustomAttributes<RouteAttribute>().Select(ra => ra.Name).FirstOrDefault(),
					q = controllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().GetActionMapping(controllerContext.ControllerDescriptor).SelectMany(g => g).ToArray(),
					Params = ad.ParameterDescriptions,
				})
				.Select(o => new ActionLink(o.RouteName, o.Api.HttpMethod.Method, controllerContext.RequestContext.Url.Link(o.RouteName, null)))
				.ToArray();
		}
	}
}