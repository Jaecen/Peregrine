using System;
using System.Collections.Generic;
using System.Linq;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Models
{
	public class StandingsResponseProvider
	{
		readonly StatsProvider StatsProvider;

		public StandingsResponseProvider(StatsProvider statsProvider)
		{
			if(statsProvider == null)
				throw new ArgumentNullException("statsProvider");

			StatsProvider = statsProvider;
		}

		public StandingsResponse Create(Tournament tournament, int? roundNumber)
		{
			if(tournament == null)
				throw new ArgumentNullException("tournament");

			var playerStandings = tournament
				.Players
				.Select(player => new
				{
					name = player.Name,
					matchPoints = StatsProvider.GetMatchPoints(tournament, player, roundNumber),
					matchWinPercentage = StatsProvider.GetMatchWinPercentage(tournament, player, roundNumber),
					opponentsMatchWinPercentage = StatsProvider.GetOpponentsMatchWinPercentage(tournament, player, roundNumber),
					gamePoints = StatsProvider.GetGamePoints(tournament, player, roundNumber),
					gameWinPercentage = StatsProvider.GetGameWinPercentage(tournament, player, roundNumber),
					opponentsGameWinPercentage = StatsProvider.GetOpponentsGameWinPercentage(tournament, player, roundNumber),
				})
				.OrderByDescending(o => o.matchPoints)
				.ThenByDescending(o => o.opponentsMatchWinPercentage)
				.ThenByDescending(o => o.gameWinPercentage)
				.ThenByDescending(o => o.opponentsGameWinPercentage)
				.Select((o, rank) => new PlayerStandingResponse(
					rank: rank + 1,
					playerName: o.name,
					matchPoints: o.matchPoints,
					matchWinPercentage: o.matchWinPercentage,
					opponentsMatchWinPercentage: o.opponentsMatchWinPercentage,
					gamePoints: o.gamePoints,
					gameWinPercentage: o.gameWinPercentage,
					opponentsGameWinPercentage: o.opponentsGameWinPercentage
				))
				.ToArray();

			return new StandingsResponse(playerStandings);
		}
	}
}