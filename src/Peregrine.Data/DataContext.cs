using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Peregrine.Data
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class User : IdentityUser
	{
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
			// Add custom user claims here
			return userIdentity;
		}
	}
	
	public class IdentityDataContext : IdentityDbContext<User>
	{
		public IdentityDataContext()
			: base(DataContext.ConnectionStringName, throwIfV1Schema: false)
		{ }

		public static IdentityDataContext Create()
		{
			return new IdentityDataContext();
		}
	}

	public class DataContext : DbContext
	{
		public const string ConnectionStringName = "Peregrine.Data.DataContext";

		public virtual IDbSet<Tournament> Tournaments { get; set; }
		public virtual IDbSet<Game> Games { get; set; }
		public virtual IDbSet<Player> Players { get; set; }
		public virtual IDbSet<Round> Rounds { get; set; }
		public virtual IDbSet<Match> Matches { get; set; }

		public DataContext()
			: base(ConnectionStringName)
		{ }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Match>()
				.HasMany(c => c.Players)
				.WithMany()                 // Note the empty WithMany()
				.Map(x =>
				{
					x.MapLeftKey("MatchId");
					x.MapRightKey("PlayerId");
					x.ToTable("Match_Players");
				});

			base.OnModelCreating(modelBuilder);
		}
	}
}
