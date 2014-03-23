using System;
using System.Collections.Generic;

namespace Peregrine.Data
{
	public class Tournament
	{
		public const int GamesPerRound = 3;
		public const int MinGamesPerRound = 2;

		public virtual int Id { get; set; }
		public virtual Guid Key { get; set; }
		public virtual string Name { get; set; }
		public virtual int Seed { get; set; }
		public virtual ICollection<Player> Players { get; set; }
		public virtual ICollection<Round> Rounds { get; set; }
	}
}
