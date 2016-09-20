using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Peregrine.Data;

namespace Peregrine.Web.Services
{
	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store, IDataProtector dataProtector)
			: base(store)
		{
			UserValidator = new UserValidator<ApplicationUser>(this)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtector);

			PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};
		}
	}
}
