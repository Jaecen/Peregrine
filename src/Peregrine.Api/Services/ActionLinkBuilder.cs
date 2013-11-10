using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using Peregrine.Api.Model;

namespace Peregrine.Api.Services
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
					Params = ad.ParameterDescriptions.Where(pd => !controllerContext.RouteData.Values.ContainsKey(pd.Name)).Select(pd => pd.Name),
				})
				.Select(o => new ActionLink(
					o.RouteName,
					o.Api.HttpMethod.Method,
					controllerContext.RequestContext.Url.Link(o.RouteName, o.Params.ToDictionary<string, string, object>(param => param, param => String.Format("!{0}!", param))),
					o.Params)
				)
				.ToArray();
		}
	}
}