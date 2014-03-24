using System.Collections.Generic;

namespace Peregrine.Data
{
	[System.Diagnostics.DebuggerDisplay("Match {Id}")]
	public class Match
	{
		public virtual int Id { get; set; }
		public virtual int Number { get; set; }
		public virtual ICollection<Player> Players { get; set; }
		public virtual ICollection<Game> Games { get; set; }
	}
}