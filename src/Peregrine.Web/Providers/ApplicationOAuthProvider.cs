﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Peregrine.Web.Services;
using System.Security.Claims;
using Peregrine.Data;
using System.Linq;

namespace Peregrine.Web.Providers
{
	public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
	{
		readonly string PublicClientId;
		readonly Func<ApplicationUserManager> UserManagerFactory;

		public ApplicationOAuthProvider(string publicClientId, Func<ApplicationUserManager> userManagerFactory)
		{
			if(publicClientId == null)
				throw new ArgumentNullException("publicClientId");

			PublicClientId = publicClientId;
			UserManagerFactory = userManagerFactory;
		}
		
		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			var userManager = UserManagerFactory();

			var user = await userManager.FindAsync(context.UserName, context.Password);
			if(user == null)
			{
				context.SetError("invalid_grant", "The user name or password is incorrect.");
				return;
			}

			var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
			var cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);

			var properties = CreateProperties(user.UserName);
			var ticket = new AuthenticationTicket(oAuthIdentity, properties);
			context.Validated(ticket);
			context.Request.Context.Authentication.SignIn(cookiesIdentity);
		}

		public override Task TokenEndpoint(OAuthTokenEndpointContext context)
		{
			foreach(KeyValuePair<string, string> property in context.Properties.Dictionary)
				context.AdditionalResponseParameters.Add(property.Key, property.Value);

			return Task.FromResult<object>(null);
		}

		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			// Resource owner password credentials does not provide a client ID.
			if(context.ClientId == null)
				context.Validated();

			return Task.FromResult<object>(null);
		}

		public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
		{
			if(context.ClientId == PublicClientId)
			{
				var expectedRootUri = new Uri(context.Request.Uri, "/");
				if(context.RedirectUri.StartsWith(expectedRootUri.AbsoluteUri))
					context.Validated();
			}

			return Task.FromResult<object>(null);
		}

		public static AuthenticationProperties CreateProperties(string userName)
		{
			return new AuthenticationProperties(new Dictionary<string, string>
				{
					{ "userName", userName }
				});
		}

	}
}