using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Peregrine.Service
{
	public class ResourceActionResult : IHttpActionResult
	{
		readonly IHttpActionResult InnerActionResult;
		readonly IEnumerable<HttpMethod> AvailableHttpMethods;
		readonly IDictionary<string, Uri> RelatedResources;

		public ResourceActionResult(HttpControllerContext controllerContext, IHttpActionResult innerActionResult, IEnumerable<KeyValuePair<string, Uri>> relatedResources = null)
		{
			if(innerActionResult == null)
				throw new ArgumentNullException("innerActionResult");

			if(controllerContext == null)
				throw new ArgumentNullException("controllerContext");

			InnerActionResult = innerActionResult;

			// I have no idea what I'm doing aside from violating the Law of Demeter in the most obscene sense imaginable.
			AvailableHttpMethods = controllerContext
				.ControllerDescriptor
				.Configuration
				.Services
				.GetActionSelector()
				.GetActionMapping(controllerContext.ControllerDescriptor)	// That's right: we just passed a reference to something we dotted down from about five lines ago
				.SelectMany(actionDescriptorGroup => actionDescriptorGroup	// LINQ: when dotting through a single chain of objects just isn't enough.
					.SelectMany(actionDescriptor => actionDescriptor		// Nested SelectMany's: when destroying the sanity of a single programmer just isn't enough.
						.SupportedHttpMethods								// Honestly, I've forgotten what I even came down here for. This looks good enough.
					)
				)
				.Distinct()
				.ToArray();

			RelatedResources = (relatedResources ?? Enumerable.Empty<KeyValuePair<string, Uri>>()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			return InnerActionResult.ExecuteAsync(cancellationToken)
				.ContinueWith(innerResponse =>
				{
					var acceptHeader = String.Join(",", AvailableHttpMethods);
					var linkHeader = String.Join(",", RelatedResources.Select(kvp => String.Format("<{0}>; rel=\"{1}\"", kvp.Value, kvp.Key)));

					innerResponse.Result.Headers.Add("Accept", acceptHeader);
					innerResponse.Result.Headers.Add("Link", linkHeader);

					return innerResponse;
				})
				.Unwrap();
		}
	}
}