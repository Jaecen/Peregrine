using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Providers;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("user")]
	public class UserController : ApiController
	{
		readonly ApplicationUserManager UserManager;
		readonly ExternalLoginContextProvider ExternalLoginContextProvider;

		public UserController(ApplicationUserManager userManager, ExternalLoginContextProvider externalLoginContextProvider)
		{
			UserManager = userManager;
			ExternalLoginContextProvider = externalLoginContextProvider;
		}

		[Route]
		public UserInfoViewModel Get()
		{
			var externalLogin = ExternalLoginContextProvider.CreateContextFromIdentity(User.Identity as ClaimsIdentity);

			return new UserInfoViewModel
				{
					Email = User.Identity.GetUserName(),
					HasRegistered = externalLogin == null,
					LoginProvider = externalLogin != null
						? externalLogin.LoginProvider
						: null
				};
		}

		[Route]
		public IHttpActionResult Put(RegisterBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = new User
			{
				UserName = model.Email,
				Email = model.Email
			};

			var result = UserManager.CreateAsync(user, model.Password).Result;

			return result.Succeeded
				? Ok()
				: GetErrorResult(result);
		}

		IHttpActionResult GetErrorResult(IdentityResult result)
		{
			if(result.Errors != null)
				foreach(string error in result.Errors)
					ModelState.AddModelError("", error);

			if(ModelState.IsValid)
				// No ModelState errors are available to send, so just return an empty BadRequest.
				return BadRequest();

			return BadRequest(ModelState);
		}
	}
}