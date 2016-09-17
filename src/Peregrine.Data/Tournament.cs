using System;
using System.Collections.Generic;

namespace Peregrine.Data
{
	[System.Diagnostics.DebuggerDisplay("Tournament {Key}")]
	public class Tournament
	{
		public virtual int Id { get; set; }
		public virtual Guid Key { get; set; }
		public virtual string Name { get; set; }
		public virtual int Seed { get; set; }
		public virtual int? ActiveRoundNumber { get; set; }
		public virtual ICollection<Player> Players { get; set; }
		public virtual ICollection<ApplicationUser> Organizers { get; set; }
		public virtual ICollection<Round> Rounds { get; set; }
	}
}
