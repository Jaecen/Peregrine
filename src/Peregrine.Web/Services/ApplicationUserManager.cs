using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Peregrine.Data;

namespace Peregrine.Web.Services
{
	public class ApplicationUserManager : UserManager<User>
	{
		public ApplicationUserManager(IUserStore<User> store, IdentityFactoryOptions<ApplicationUserManager> options)
			: base(store)
		{
			// Configure validation logic for usernames
			UserValidator = new UserValidator<User>(this)
				{
					AllowOnlyAlphanumericUserNames = false,
					RequireUniqueEmail = true
				};

			// Configure validation logic for passwords
			PasswordValidator = new PasswordValidator
				{
					RequiredLength = 6,
					//RequireNonLetterOrDigit = true,
					//RequireDigit = true,
					//RequireLowercase = true,
					//RequireUppercase = true,
				};

			var dataProtectionProvider = options.DataProtectionProvider;
			if(dataProtectionProvider != null)
				UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
		}
	}
}
