using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace Peregrine.Web.Services
{
	public class ExternalLoginContext
	{
		public readonly string LoginProvider;
		public readonly string ProviderKey;
		public readonly string UserName;

		public ExternalLoginContext(string loginProvider, string providerKey, string userName)
		{
			LoginProvider = loginProvider;
			ProviderKey = providerKey;
			UserName = userName;
		}
	}

	public class ExternalLoginContextProvider
	{
		public ExternalLoginContext CreateContextFromIdentity(ClaimsIdentity identity)
		{
			if(identity == null)
				return null;

			var providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

			if(providerKeyClaim == null
				|| String.IsNullOrEmpty(providerKeyClaim.Issuer)
				|| String.IsNullOrEmpty(providerKeyClaim.Value))
				return null;

			if(providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
				return null;

			return new ExternalLoginContext(
					loginProvider: providerKeyClaim.Issuer,
					providerKey: providerKeyClaim.Value,
					userName: identity.FindFirstValue(ClaimTypes.Name));
		}

		public IEnumerable<Claim> GetClaimsFromContext(ExternalLoginContext context)
		{
			yield return new Claim(ClaimTypes.NameIdentifier, context.ProviderKey, null, context.LoginProvider);

			if(context.UserName != null)
				yield return new Claim(ClaimTypes.Name, context.UserName, null, context.LoginProvider);
		}

	}
}