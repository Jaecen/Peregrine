﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Peregrine.Web.Models
{
	public class AddExternalLoginBindingModel
	{
		[Required]
		[Display(Name = "External access token")]
		public string ExternalAccessToken { get; set; }
	}

	public class ChangePasswordBindingModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }
	}

	public class RegisterBindingModel
	{
		[Required]
		[Display(Name = "Email")]
		public string email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string password { get; set; }
	}

	public class RegisterExternalBindingModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class RemoveLoginBindingModel
	{
		[Required]
		[Display(Name = "Login provider")]
		public string LoginProvider { get; set; }

		[Required]
		[Display(Name = "Provider key")]
		public string ProviderKey { get; set; }
	}

	public class SetPasswordBindingModel
	{
		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }
	}

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
}