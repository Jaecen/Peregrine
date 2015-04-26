using System;
using System.Collections.Generic;
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
using Microsoft.Owin.Security.OAuth;
using Peregrine.Web.Models;
using Peregrine.Web.Providers;
using Peregrine.Web.Results;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Peregrine.Data;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.DataHandler;

namespace Peregrine.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
		private AuthRepository AuthRepo;

        public AccountController()
        {
			AuthRepo = new AuthRepository();
			UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>());
			AccessTokenFormat = new TicketDataFormat(new DpapiDataProtectionProvider().Create());
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
		[AllowAnonymous]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
			Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

			//var redirectUri =
			//	Request.GetQueryNameValuePairs()
			//	.Where(param => param.Key.Equals("redirect_uri", StringComparison.OrdinalIgnoreCase))
			//	.FirstOrDefault()
			//	.Value;

			//if(String.IsNullOrEmpty(redirectUri))
			//	redirectUri = String.Empty;



			Uri expectedRootUri = new Uri(HttpContext.Current.Request.Url, "/");
			
			var redirectUri = string.Format("{0}#/login/?externalAccessToken={1}&provider={2}&hasLocalAccount={3}&externalUserName={4}",
											expectedRootUri.AbsoluteUri,
											externalLogin.ExternalAccessToken,
											externalLogin.LoginProvider,
											hasRegistered.ToString(),
											externalLogin.UserName);

			return Redirect(redirectUri);
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

		// POST api/Account/RegisterExternal
		[AllowAnonymous]
		[Route("RegisterExternal")]
		public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
		{

			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
			if(verifiedAccessToken == null)
			{
				return BadRequest("Invalid Provider or External Access Token");
			}

			//IdentityUser user = await AuthRepo.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));
			IdentityUser user = AuthRepo.GetUser(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

			bool hasRegistered = user != null;

			if(hasRegistered)
			{
				return BadRequest("External user is already registered");
			}

			user = new IdentityUser() { UserName = model.UserName };

			IdentityResult result = await AuthRepo.CreateAsync(user);
			if(!result.Succeeded)
			{
				return GetErrorResult(result);
			}

			var info = new ExternalLoginInfo()
			{
				DefaultUserName = model.UserName,
				Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
			};

			result = await AuthRepo.AddLoginAsync(user.Id, info.Login);
			if(!result.Succeeded)
			{
				return GetErrorResult(result);
			}

			//generate access token response
			var accessTokenResponse = GenerateLocalAccessTokenResponse(model.UserName);

			return Ok(accessTokenResponse);
		}

		private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
		{
			ParsedExternalAccessToken parsedToken = null;

			var verifyTokenEndPoint = "";

			if(provider == "Facebook")
			{
				//You can get it from here: https://developers.facebook.com/tools/accesstoken/
				//More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
				var appToken = "xxxxxx";
				verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
			}
			else if(provider == "Google")
			{
				verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
			}
			else
			{
				return null;
			}

			var client = new HttpClient();
			var uri = new Uri(verifyTokenEndPoint);
			var response = await client.GetAsync(uri);

			if(response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();

				dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

				parsedToken = new ParsedExternalAccessToken();

				if(provider == "Facebook")
				{
					parsedToken.user_id = jObj["data"]["user_id"];
					parsedToken.app_id = jObj["data"]["app_id"];

					if(!string.Equals(ConfigurationManager.AppSettings["FacebookAppId"], parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
					{
						return null;
					}
				}
				else if(provider == "Google")
				{
					parsedToken.user_id = jObj["user_id"];
					parsedToken.app_id = jObj["audience"];

					if(!string.Equals(ConfigurationManager.AppSettings["GoogleClientId"], parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
					{
						return null;
					}

				}

			}

			return parsedToken;
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("ObtainLocalAccessToken")]
		public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
		{

			if(string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
			{
				return BadRequest("Provider or external access token is not sent");
			}

			var info = await Authentication.GetExternalLoginInfoAsync();
			if(info == null)
			{
				return InternalServerError();
			}
			IdentityUser user = await _userManager.FindAsync(info.Login);

			bool hasRegistered = user != null;

			if(!hasRegistered)
			{
				return BadRequest("External user is not registered");
			}

			//generate access token response
			var accessTokenResponse = GenerateLocalAccessTokenResponse(user.UserName);

			return Ok(accessTokenResponse);

		}

		private JObject GenerateLocalAccessTokenResponse(string userName)
		{

			var tokenExpiration = TimeSpan.FromDays(1);

			ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

			identity.AddClaim(new Claim(ClaimTypes.Name, userName));
			identity.AddClaim(new Claim("role", "user"));

			var props = new AuthenticationProperties()
			{
				IssuedUtc = DateTime.UtcNow,
				ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
			};

			var ticket = new AuthenticationTicket(identity, props);

			var accessToken = AccessTokenFormat.Protect(ticket);

			JObject tokenResponse = new JObject(
										new JProperty("userName", userName),
										new JProperty("access_token", accessToken),
										new JProperty("token_type", "bearer"),
										new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
										new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
										new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
									);

			return tokenResponse;
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
			public string UserName { get; set; }
			public string ExternalAccessToken { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
					ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken")
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
