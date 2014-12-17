using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Results;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[Authorize]
	[RoutePrefix("api/account")]
	public class AccountController : ApiController
	{
		const int ExternalLoginStateStrengthInBits = 256;
		const string LocalLoginProvider = "Local";

		readonly ApplicationUserManager UserManager;
		readonly ISecureDataFormat<AuthenticationTicket> AccessTokenFormat;
		readonly RandomNumberGenerator Rng;
		readonly ExternalLoginContextProvider ExternalLoginContextProvider;

		public AccountController()
		{
			UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
			Rng = new RNGCryptoServiceProvider();
			ExternalLoginContextProvider = new ExternalLoginContextProvider();
		}

		public AccountController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
		{
			UserManager = userManager;
			AccessTokenFormat = accessTokenFormat;
			Rng = new RNGCryptoServiceProvider();
		}

		// GET api/Account/UserInfo
		[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
		[Route("userInfo")]
		public UserInfoViewModel GetUserInfo()
		{
			var externalLogin = ExternalLoginContextProvider.CreateContextFromIdentity(User.Identity as ClaimsIdentity);

			return new UserInfoViewModel
				{
					Email = User.Identity.GetUserName(),
					HasRegistered = externalLogin == null,
					LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
				};
		}

		// POST api/Account/Logout
		[Route("logout")]
		public IHttpActionResult Logout()
		{
			Request
				.GetOwinContext()
				.Authentication
				.SignOut(CookieAuthenticationDefaults.AuthenticationType);

			return Ok();
		}

		// GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
		[Route("manageInfo")]
		public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
		{
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if(user == null)
				return null;

			var logins = user.Logins
				.Select(linkedAccount => new UserLoginInfoViewModel
					{
						LoginProvider = linkedAccount.LoginProvider,
						ProviderKey = linkedAccount.ProviderKey
					})
				.Concat(user.PasswordHash != null
					? new[]
						{
							new UserLoginInfoViewModel
								{
									LoginProvider = LocalLoginProvider,
									ProviderKey = user.UserName,
								}
						}
					: Enumerable.Empty<UserLoginInfoViewModel>());

			return new ManageInfoViewModel
			{
				LocalLoginProvider = LocalLoginProvider,
				Email = user.UserName,
				Logins = logins,
				ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
			};
		}

		// POST api/Account/ChangePassword
		[Route("changePassword")]
		public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

			return result.Succeeded
				? Ok()
				: GetErrorResult(result);
		}

		// POST api/Account/SetPassword
		[Route("setPassword")]
		public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

			return result.Succeeded
				? Ok()
				: GetErrorResult(result);
		}

		// POST api/Account/AddExternalLogin
		[Route("addExternalLogin")]
		public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			Request
				.GetOwinContext()
				.Authentication
				.SignOut(DefaultAuthenticationTypes.ExternalCookie);

			var authenticationTicket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

			if(authenticationTicket == null
				|| authenticationTicket.Identity == null
				|| (
					authenticationTicket.Properties != null
					&& authenticationTicket.Properties.ExpiresUtc.HasValue
					&& authenticationTicket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
				return BadRequest("External login failure.");

			var externalLoginData = ExternalLoginContextProvider.CreateContextFromIdentity(authenticationTicket.Identity);
			if(externalLoginData == null)
				return BadRequest("The external login is already associated with an account.");

			var result = await UserManager.AddLoginAsync(
				User.Identity.GetUserId(),
				new UserLoginInfo(externalLoginData.LoginProvider, externalLoginData.ProviderKey));

			return result.Succeeded
				? Ok()
				: GetErrorResult(result);
		}

		// POST api/Account/RemoveLogin
		[Route("removeLogin")]
		public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = model.LoginProvider == LocalLoginProvider
				? await UserManager.RemovePasswordAsync(User.Identity.GetUserId())
				: await UserManager.RemoveLoginAsync(
					User.Identity.GetUserId(),
					new UserLoginInfo(model.LoginProvider, model.ProviderKey));

			return result.Succeeded
				? Ok()
				: GetErrorResult(result);
		}

		// GET api/Account/ExternalLogin
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
		[AllowAnonymous]
		[Route("externalLogin", Name = "ExternalLogin")]
		public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
		{
			if(error != null)
				return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));

			if(!User.Identity.IsAuthenticated)
				return new ChallengeResult(provider, this);

			var externalLoginData = ExternalLoginContextProvider.CreateContextFromIdentity(User.Identity as ClaimsIdentity);

			if(externalLoginData == null)
				return InternalServerError();

			if(externalLoginData.LoginProvider != provider)
			{
				Request
					.GetOwinContext()
					.Authentication
					.SignOut(DefaultAuthenticationTypes.ExternalCookie);

				return new ChallengeResult(provider, this);
			}

			var user = await UserManager.FindAsync(new UserLoginInfo(externalLoginData.LoginProvider, externalLoginData.ProviderKey));

			if(user != null)
			{
				Request
					.GetOwinContext()
					.Authentication
					.SignOut(DefaultAuthenticationTypes.ExternalCookie);

				var oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager, OAuthDefaults.AuthenticationType);
				var cookieIdentity = await user.GenerateUserIdentityAsync(UserManager, CookieAuthenticationDefaults.AuthenticationType);

				var authenticationProperties = ApplicationOAuthProvider.CreateProperties(user.UserName);
				Request
					.GetOwinContext()
					.Authentication
					.SignIn(authenticationProperties, oAuthIdentity, cookieIdentity);
			}
			else
			{
				var claims = ExternalLoginContextProvider.GetClaimsFromContext(externalLoginData);
				var claimsIdentity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
				Request
					.GetOwinContext()
					.Authentication
					.SignIn(claimsIdentity);
			}

			return Ok();
		}

		// GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
		[AllowAnonymous]
		[Route("externalLogins")]
		public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
		{
			var authenticationDescriptions = Request
				.GetOwinContext()
				.Authentication
				.GetExternalAuthenticationTypes();

			string state = null;
			if(generateState)
			{
				var randomData = new byte[ExternalLoginStateStrengthInBits / 8];
				Rng.GetBytes(randomData);
				state = HttpServerUtility.UrlTokenEncode(randomData);
			}

			var logins = authenticationDescriptions
				.Select(description => new ExternalLoginViewModel
					{
						Name = description.Caption,
						Url = Url.Route("ExternalLogin", new
						{
							provider = description.AuthenticationType,
							response_type = "token",
							client_id = AuthConfig.PublicClientId,
							redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
							state = state
						}),
						State = state
					});

			return logins;
		}

		// POST api/Account/Register
		[AllowAnonymous]
		[Route("Register")]
		public async Task<IHttpActionResult> Register(RegisterBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = new User
				{
					UserName = model.email,
					Email = model.email
				};

			var result = await UserManager.CreateAsync(user, model.password);

			return result.Succeeded
				? Ok()
				: GetErrorResult(result);
		}

		// POST api/Account/RegisterExternal
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
		[Route("RegisterExternal")]
		public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var info = await Request
				.GetOwinContext()
				.Authentication
				.GetExternalLoginInfoAsync();

			if(info == null)
				return InternalServerError();

			var user = new User()
				{
					UserName = model.Email,
					Email = model.Email
				};

			var createResult = await UserManager.CreateAsync(user);

			if(!createResult.Succeeded)
				return GetErrorResult(createResult);

			var addLoginResult = await UserManager.AddLoginAsync(user.Id, info.Login);

			return addLoginResult.Succeeded
				? Ok()
				: GetErrorResult(addLoginResult);
		}

		IHttpActionResult GetErrorResult(IdentityResult result)
		{
			if(result == null)
				return InternalServerError();

			if(result.Succeeded)
				return null;

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
