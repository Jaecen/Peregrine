using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Peregrine.Data.Migrations
{
	internal sealed class Configuration : DbMigrationsConfiguration<Peregrine.Data.DataContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(DataContext context)
		{
			AddAdminRole(context);
			AddJason(context);
			AddScott(context);
			AddNonAdmin(context);
			AddDefaultClients(context);
		}
		void AddAdminRole(DataContext context)
		{
			if(context.Roles.Where(r => r.Name == "Admin").Any())
				return;

			var store = new RoleStore<IdentityRole>(context);
			var manager = new RoleManager<IdentityRole>(store);
			var role = new IdentityRole
			{
				Name = "Admin"
			};

			manager.Create(role);
		}

		void AddScott(DataContext context)
		{
			if(context.Users.Any(u => u.UserName == "friendscottn@gmail.com"))
				return;

			var store = new UserStore<ApplicationUser>(context);
			var manager = new UserManager<ApplicationUser>(store);
			var user = new ApplicationUser
			{
				UserName = "friendscottn@gmail.com",
				Email = "friendscottn@gmail.com"
			};

			manager.Create(
				user: user,
				password: "ChangeIt!1");

			manager.AddToRole(user.Id, "Admin");
		}

		void AddNonAdmin(DataContext context)
		{
			if(context.Users.Any(u => u.UserName == "nonadmin@magictourney.com"))
				return;

			var store = new UserStore<ApplicationUser>(context);
			var manager = new UserManager<ApplicationUser>(store);
			var user = new ApplicationUser
			{
				UserName = "nonadmin@magictourney.com",
				Email = "nonadmin@magictourney.com"
			};

			manager.Create(
				user: user,
				password: "ChangeIt!1");
		}

		void AddJason(DataContext context)
		{
			if(context.Users.Any(u => u.UserName == "jaecen@gmail.com"))
				return;

			var store = new UserStore<ApplicationUser>(context);
			var manager = new UserManager<ApplicationUser>(store);
			var user = new ApplicationUser
			{
				UserName = "jaecen@gmail.com",
				Email = "jaecen@gmail.com"
			};

			manager.Create(
				user: user,
				password: "ChangeIt!1");

			manager.AddToRole(user.Id, "Admin");
		}

		void AddDefaultClients(DataContext context)
		{
			if(context.Clients.Count() > 0)
				return;

			context.Clients.AddRange(BuildClientsList());
			context.SaveChanges();
		}

		private static List<Client> BuildClientsList()
		{
			List<Client> ClientsList = new List<Client> 
            {
                new Client
                { Id = "ngAuthApp", 
                    Secret= Helper.GetHash("abc@123"), 
                    Name="AngularJS front-end Application", 
                    ApplicationType =  ApplicationTypes.JavaScript, 
                    Active = true, 
                    RefreshTokenLifeTime = 7200, 
                    AllowedOrigin = "http://ngauthenticationweb.azurewebsites.net"
                },
                new Client
                { Id = "consoleApp", 
                    Secret=Helper.GetHash("123@abc"), 
                    Name="Console Application", 
                    ApplicationType = ApplicationTypes.NativeConfidential, 
                    Active = true, 
                    RefreshTokenLifeTime = 14400, 
                    AllowedOrigin = "*"
                }
            };

			return ClientsList;
		}
	}
}
