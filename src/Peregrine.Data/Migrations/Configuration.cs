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
	}
}
