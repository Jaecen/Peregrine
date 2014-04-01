using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peregrine.Web.Models
{
	public class PlayerMatchStatsResponse
	{
		public readonly string name;
		public readonly bool dropped;
		public readonly int wins;
		public readonly int losses;
		public readonly int draws;

		public PlayerMatchStatsResponse(string name, bool dropped, int wins, int losses, int draws)
		{
			if(String.IsNullOrEmpty(name))
				throw new ArgumentException("Must not be null or empty", "name");

			this.name = name;
			this.dropped = dropped;
			this.wins = wins;
			this.losses = losses;
			this.draws = draws;
		}
	}
}