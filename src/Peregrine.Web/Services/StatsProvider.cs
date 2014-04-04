using System;
using System.Linq;
using Peregrine.Data;

namespace Peregrine.Web.Services
{
	public class StatsProvider
	{
		const int MatchWin = 3;
		const int MatchDraw = 1;
		const int MatchLoss = 0;
		const int GameWin = 3;
		const int GameDraw = 1;
		const int GameLoss = 0;

		class PlayerStats
		{
			public readonly int Wins;
			public readonly int Losses;
			public readonly int Draws;

			public PlayerStats(int wins, int losses, int draws)
			{
				Wins = wins;
				Losses = losses;
				Draws = draws;
			}
		}

		public int GetMatchPoints(Tournament tournament, Player player, int? roundNumber = null)
		{
			return tournament
				.GetPlayerMatches(player, roundNumber)
				.Select(match => new PlayerStats(
					wins: match.Games.Where(game => game.Winner == player).Count(),
					losses: match.Games.Where(game => game.Winner != player && game.Winner != null).Count(),
					draws: match.Games.Where(game => game.Winner == player).Count()
				))
				.Select(stats => stats.Wins > stats.Losses
					? MatchWin
					: stats.Wins == stats.Losses
						? MatchDraw
						: MatchLoss)
				.Sum();
		}

		public int GetGamePoints(Tournament tournament, Player player, int? roundNumber = null)
		{
			return tournament
				.GetPlayerGames(player, roundNumber)
				.Select(game => new PlayerStats(
					wins: game.Winner == player ? 1 : 0,
					losses: game.Winner != player && game.Winner != null ? 1 : 0,
					draws: game.Winner == null ? 1 : 0
				))
				.Aggregate(0, (sum, stats) => sum + (stats.Wins * GameWin + stats.Draws * GameDraw + stats.Losses * GameLoss));
		}

		public decimal GetMatchWinPercentage(Tournament tournament, Player player, int? roundNumber = null)
		{
			var achieved = GetMatchPoints(tournament, player, roundNumber);

			var maximum = tournament
				.GetPlayerMatches(player, roundNumber)
				.Count() * MatchWin;

			if(maximum == 0)
				return 0.33m;

			var rawPercentage = achieved / (decimal)maximum;

			// Match win percentage is capped at 0.33 on the low end
			return Math.Max(0.33m, rawPercentage);
		}

		public decimal GetGameWinPercentage(Tournament tournament, Player player, int? roundNumber = null)
		{
			var achieved = GetGamePoints(tournament, player, roundNumber);

			var maximum = tournament
				.GetPlayerGames(player, roundNumber)
				.Count() * GameWin;

			if(maximum == 0)
				return 0.33m;

			var rawPercentage = achieved / (decimal)maximum;

			// Game win percentage is capped at 0.33 on the low end
			return Math.Max(0.33m, rawPercentage);
		}

		public decimal GetOpponentsMatchWinPercentage(Tournament tournament, Player player, int? roundNumber = null)
		{
			var opponents = tournament.GetPlayerOpponents(player, roundNumber);

			return opponents
				.Select(opponent => GetMatchWinPercentage(tournament, opponent, roundNumber))
				.DefaultIfEmpty(0)
				.Average();
		}

		public decimal GetOpponentsGameWinPercentage(Tournament tournament, Player player, int? roundNumber = null)
		{
			var opponents = tournament.GetPlayerOpponents(player, roundNumber);

			return opponents
				.Select(opponent => GetGameWinPercentage(tournament, opponent, roundNumber))
				.DefaultIfEmpty(0)
				.Average();
		}
	}
}