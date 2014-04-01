using System;
using System.Linq;
using Peregrine.Data;

namespace Peregrine.Web.Models
{
	public class MatchResponseProvider
	{
		readonly PlayerMatchStatsResponseProvider PlayerMatchStatsResponseProvider;

		public MatchResponseProvider(PlayerMatchStatsResponseProvider playerMatchStatsResponseProvider)
		{
			if(playerMatchStatsResponseProvider == null)
				throw new ArgumentNullException("playerMatchStatsResponseProvider");

			PlayerMatchStatsResponseProvider = playerMatchStatsResponseProvider;
		}

		public MatchResponse Create(Match match)
		{
			if(match == null)
				throw new ArgumentNullException("match");

			return new MatchResponse(
				number: match.Number,
				gamesPlayed: match.Games.Count,
				players: match
					.Players
					.OrderBy(player => player.Name)
					.Select(player => PlayerMatchStatsResponseProvider.Create(player, match))
					.ToArray()
			);
		}
	}
}