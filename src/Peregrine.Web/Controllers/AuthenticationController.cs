using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.Cookies;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/authentication")]
	public class AuthenticationController : ApiController
	{
		[Route]
		public IHttpActionResult Delete()
		{
			Request
				.GetOwinContext()
				.Authentication
				.SignOut(CookieAuthenticationDefaults.AuthenticationType);

			return Ok();
		}
	}
}