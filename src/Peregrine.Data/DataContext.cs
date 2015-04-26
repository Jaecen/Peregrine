using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Peregrine.Data
{
	public class DataContext : IdentityDbContext<IdentityUser>
	{
		public virtual IDbSet<Tournament> Tournaments { get; set; }
		public virtual IDbSet<Game> Games { get; set; }
		public virtual IDbSet<Player> Players { get; set; }
		public virtual IDbSet<Round> Rounds { get; set; }
		public virtual IDbSet<ApplicationUser> Users { get; set; }
		public virtual DbSet<Client> Clients { get; set; }
		public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

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
