using System.Collections.Generic;
using System.Linq;

namespace Peregrine.Web.Models
{
	public class MatchResponse
	{
		public readonly int number;
		public readonly int gamesPlayed;
		public readonly IEnumerable<PlayerMatchStatsResponse> players;

		public MatchResponse(int number, int gamesPlayed, IEnumerable<PlayerMatchStatsResponse> players)
		{
			this.number = number;
			this.gamesPlayed = gamesPlayed;
			this.players = players ?? Enumerable.Empty<PlayerMatchStatsResponse>();
		}
	}
}