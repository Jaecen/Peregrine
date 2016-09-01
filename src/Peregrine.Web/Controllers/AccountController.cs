using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
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
		const string LocalLoginProvider = "Local";

		readonly ApplicationUserManager UserManager;
		readonly ISecureDataFormat<AuthenticationTicket> AccessTokenFormat;

		IAuthenticationManager Authentication
		{ get { return Request.GetOwinContext().Authentication; } }

		public AccountController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
		{
			UserManager = userManager; // new ApplicationUserManager(new UserStore<ApplicationUser>(new DataContext()));
			AccessTokenFormat = accessTokenFormat; 
		}

		// GET api/account/UserInfo
		[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
		[Route("UserInfo")]
		public UserInfoViewModel GetUserInfo()
		{
			var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

			return new UserInfoViewModel
			{
				Email = User.Identity.GetUserName(),
				HasRegistered = externalLogin == null,
				LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
			};
		}

		// POST api/account/Logout
		[Route("Logout")]
		[AllowAnonymous]
		public IHttpActionResult Logout()
		{
			Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
			Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
			return Ok();
		}

		// GET api/account/ManageInfo?returnUrl=%2F&generateState=true
		[Route("ManageInfo")]
		public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
		{
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if(user == null)
				return null;

			var logins = new List<UserLoginInfoViewModel>();

			foreach(IdentityUserLogin linkedAccount in user.Logins)
				logins.Add(new UserLoginInfoViewModel
				{
					LoginProvider = linkedAccount.LoginProvider,
					ProviderKey = linkedAccount.ProviderKey
				});

			if(user.PasswordHash != null)
				logins.Add(new UserLoginInfoViewModel
				{
					LoginProvider = LocalLoginProvider,
					ProviderKey = user.UserName,
				});

			return new ManageInfoViewModel
			{
				LocalLoginProvider = LocalLoginProvider,
				Email = user.UserName,
				Logins = logins,
				ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
			};
		}

		// POST api/account/ChangePassword
		[Route("ChangePassword")]
		public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
			if(!result.Succeeded)
				return GetErrorResult(result);

			return Ok();
		}

		// POST api/account/SetPassword
		[Route("SetPassword")]
		public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

			if(!result.Succeeded)
				return GetErrorResult(result);

			return Ok();
		}

		// POST api/account/AddExternalLogin
		[Route("AddExternalLogin")]
		public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

			var ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);
			if(ticket == null
				|| ticket.Identity == null
				|| (ticket.Properties != null && ticket.Properties.ExpiresUtc.HasValue && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
			{
				return BadRequest("External login failure.");
			}

			var externalData = ExternalLoginData.FromIdentity(ticket.Identity);

			if(externalData == null)
				return BadRequest("The external login is already associated with an account.");

			var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));
			if(!result.Succeeded)
				return GetErrorResult(result);

			return Ok();
		}

		// POST api/account/RemoveLogin
		[Route("RemoveLogin")]
		public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = model.LoginProvider == LocalLoginProvider
				? await UserManager.RemovePasswordAsync(User.Identity.GetUserId())
				: await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(model.LoginProvider, model.ProviderKey));

			if(!result.Succeeded)
				return GetErrorResult(result);

			return Ok();
		}

		// GET api/account/ExternalLogin
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
		[AllowAnonymous]
		[Route("ExternalLogin", Name = "ExternalLogin")]
		public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
		{
			if(error != null)
				return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));

			if(!User.Identity.IsAuthenticated)
				return new ChallengeResult(provider, this);

			var userIdentity = (ClaimsIdentity)User.Identity;
			var externalLogin = ExternalLoginData.FromIdentity(userIdentity);
			if(externalLogin == null)
				return InternalServerError();

			if (externalLogin.LoginProvider != provider)
			{
				Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
				return new ChallengeResult(provider, this);
			}

			var user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));
			var hasRegistered = user != null;
			var expectedRootUri = new Uri(HttpContext.Current.Request.Url, "/");
			
			var redirectUri = string.Format(
				"{0}#/login/?externalAccessToken={1}&provider={2}&hasLocalAccount={3}&externalUserName={4}&email={5}",
				expectedRootUri.AbsoluteUri,
				externalLogin.ExternalAccessToken,
				externalLogin.LoginProvider,
				hasRegistered.ToString(),
				externalLogin.UserName,
				externalLogin.Email);

			return Redirect(redirectUri);
		}

		// GET api/account/ExternalLogins?returnUrl=%2F&generateState=true
		[AllowAnonymous]
		[Route("ExternalLogins")]
		public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
		{
			const int strengthInBits = 256;

			var state = generateState
				? RandomOAuthStateGenerator.Generate(strengthInBits)
				: null;

			var logins = Authentication
				.GetExternalAuthenticationTypes()
				.Select(description => new ExternalLoginViewModel
					{
						Name = description.Caption,
						Url = Url.Route("ExternalLogin", new
						{
							provider = description.AuthenticationType,
							response_type = "token",
							client_id = PeregrineApp.PublicClientId,
							redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
							state = state
						}),
						State = state
					})
					.ToArray();

			return logins;
		}

		// POST api/account/Register
		[AllowAnonymous]
		[Route("Register")]
		public async Task<IHttpActionResult> Register(RegisterBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = new ApplicationUser
				{
					UserName = model.Email,
					Email = model.Email
				};

			var result = await UserManager.CreateAsync(user, model.Password);
			if(!result.Succeeded)
				return GetErrorResult(result);

			return Ok();
		}

		// POST api/account/RegisterExternal
		[AllowAnonymous]
		[Route("RegisterExternal")]
		public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
			if(verifiedAccessToken == null)
				return BadRequest("Invalid Provider or External Access Token");

			var existingUser = UserManager.Find(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));
			if(existingUser != null)
				return BadRequest("External user is already registered");

			var newUser = new ApplicationUser
			{
				UserName = model.UserName,
				Email = model.Email
			};

			var createResult = await UserManager.CreateAsync(newUser);
			if(!createResult.Succeeded)
				return GetErrorResult(createResult);

			var info = new ExternalLoginInfo
				{
					DefaultUserName = model.UserName,
					Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
				};

			var addLoginResult = await UserManager.AddLoginAsync(newUser.Id, info.Login);
			if(!addLoginResult.Succeeded)
				return GetErrorResult(addLoginResult);

			var accessTokenResponse = GenerateLocalAccessTokenResponse(model.UserName);
			return Ok(accessTokenResponse);
		}

		async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
		{
			string verifyTokenEndPoint;
			if(provider == "Facebook")
			{
				//You can get it from here: https://developers.facebook.com/tools/accesstoken/
				//More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
				var appToken = ConfigurationManager.AppSettings["FacebookAppAccessToken"];
				verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
			}
			else if(provider == "Google")
				verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
			else
				return null;

			var client = new HttpClient();
			var uri = new Uri(verifyTokenEndPoint);
			var response = await client.GetAsync(uri);

			if(!response.IsSuccessStatusCode)
				return null;

			var content = await response.Content.ReadAsStringAsync();
			var jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

			if(provider == "Facebook")
			{
				var userId = (string)jObj.SelectToken("data.user_id");
				var appId = (string)jObj.SelectToken("data.app_id");

				if(!string.Equals(ConfigurationManager.AppSettings["FacebookAppId"], appId, StringComparison.OrdinalIgnoreCase))
					return null;

				return new ParsedExternalAccessToken
					{
						user_id = userId,
						app_id = appId,
					};
			}
			else if(provider == "Google")
			{
				var userId = (string)jObj["user_id"];
				var appId = (string)jObj["audience"];

				if(!string.Equals(ConfigurationManager.AppSettings["GoogleClientId"], appId, StringComparison.OrdinalIgnoreCase))
					return null;

				return new ParsedExternalAccessToken
					{
						user_id = userId,
						app_id = appId,
					};
			}
			else
				return null;
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("ObtainLocalAccessToken")]
		public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
		{
			if(string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
				return BadRequest("Provider or external access token is not sent");

			var info = await Authentication.GetExternalLoginInfoAsync();
			if(info == null)
				return InternalServerError();

			var user = await UserManager.FindAsync(info.Login);

			var hasRegistered = user != null;
			if(!hasRegistered)
				return BadRequest("External user is not registered");

			var accessTokenResponse = GenerateLocalAccessTokenResponse(user.UserName);
			return Ok(accessTokenResponse);
		}

		JObject GenerateLocalAccessTokenResponse(string userName)
		{
			var tokenExpiration = TimeSpan.FromDays(1);

			var identity = new ClaimsIdentity(
				claims: new[] 
					{
						new Claim(ClaimTypes.Name, userName),
						new Claim(ClaimTypes.Role, "user"),
					},
				authenticationType: OAuthDefaults.AuthenticationType);

			var props = new AuthenticationProperties()
				{
					IssuedUtc = DateTime.UtcNow,
					ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
				};

			var ticket = new AuthenticationTicket(identity, props);

			var accessToken = AccessTokenFormat.Protect(ticket);
			var tokenResponse = new JObject(
				new JProperty("userName", userName),
				new JProperty("access_token", accessToken),
				new JProperty("token_type", "bearer"),
				new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
				new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
				new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString()));

			return tokenResponse;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing && UserManager != null)
				UserManager.Dispose();

			base.Dispose(disposing);
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

		class ExternalLoginData
		{
			public readonly string LoginProvider;
			public readonly string ProviderKey;
			public readonly string UserName;
			public readonly string ExternalAccessToken;
			public readonly string Email;

			public ExternalLoginData(string loginProvider, string providerKey, string userName, string externalAccessToken, string email)
			{
				LoginProvider = loginProvider;
				ProviderKey = providerKey;
				UserName = userName;
				ExternalAccessToken = externalAccessToken;
				Email = email;
			}

			public IList<Claim> GetClaims()
			{
				var claims = new List<Claim>
					{
						new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider)
					};

				if(UserName != null)
					claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));

				return claims;
			}

			public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
			{
				if(identity == null)
					return null;

				var providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

				if(providerKeyClaim == null
					|| String.IsNullOrEmpty(providerKeyClaim.Issuer)
					|| String.IsNullOrEmpty(providerKeyClaim.Value))
				{
					return null;
				}

				if(providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
					return null;

				return new ExternalLoginData(
					loginProvider: providerKeyClaim.Issuer,
					providerKey: providerKeyClaim.Value,
					userName: identity.FindFirstValue(ClaimTypes.Name),
					externalAccessToken: identity.FindFirstValue("ExternalAccessToken"),
					email: identity.FindFirstValue(ClaimTypes.Email));
			}
		}

		static class RandomOAuthStateGenerator
		{
			static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

			public static string Generate(int strengthInBits)
			{
				const int bitsPerByte = 8;

				if(strengthInBits % bitsPerByte != 0)
					throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");

				var strengthInBytes = strengthInBits / bitsPerByte;
				var data = new byte[strengthInBytes];
				_random.GetBytes(data);
				return HttpServerUtility.UrlTokenEncode(data);
			}
		}
	}
}
