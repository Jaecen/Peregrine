using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Peregrine.Data;

namespace Peregrine.Web.Services
{
	class StatsManager
	{
		const int GameWin = 3;
		const int GameDraw = 1;
		const int GameLoss = 0;
		const int MatchWin = 3;
		const int MatchDraw = 1;
		const int MatchLoss = 0;

		public int GetGamePoints(Tournament tournament, Player player)
		{
			throw new NotImplementedException();
		}

		public int GetMatchPoints(Tournament tournament, Player player)
		{
			return tournament
				.Rounds
				.SelectMany(round => round.Matches)
				.Where(match => match.Players.Contains(player))
				.Select(match => new
					{
						Wins = match.Games.Where(game => game.Winner == player).Count(),
						Losses = match.Games.Where(game => game.Winner != player && game.Winner != null).Count(),
						Draws = match.Games.Where(game => game.Winner == player).Count(),
					})
				.Select(o => o.Wins > o.Losses ? MatchWin : o.Wins == o.Losses ? MatchDraw : MatchLoss)
				.Sum();
		}
	}
}