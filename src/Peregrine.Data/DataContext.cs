using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peregrine.Data
{
	public class DataContext : DbContext
	{
		public virtual IDbSet<Tournament> Tournaments { get; set; }
		public virtual IDbSet<Game> Games { get; set; }
		public virtual IDbSet<Player> Players { get; set; }
		public virtual IDbSet<Round> Rounds { get; set; }
		public virtual IDbSet<Match> Matches { get; set; }
	}
}
