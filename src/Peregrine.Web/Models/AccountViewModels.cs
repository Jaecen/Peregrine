using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peregrine.Web.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }

	namespace AngularJSAuthentication.API.Models
	{
		public class ExternalLoginViewModel
		{
			public string Name { get; set; }

			public string Url { get; set; }

			public string State { get; set; }
		}

		public class RegisterExternalBindingModel
		{
			[Required]
			public string UserName { get; set; }

			[Required]
			public string Provider { get; set; }

			[Required]
			public string ExternalAccessToken { get; set; }

		}

		public class ParsedExternalAccessToken
		{
			public string user_id { get; set; }
			public string app_id { get; set; }
		}
	}
}
