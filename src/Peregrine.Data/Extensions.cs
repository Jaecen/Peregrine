using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Peregrine.Data
{
	public static class Extensions
	{
		public static Tournament GetTournament(this DataContext dataContext, Guid tournamentKey)
		{
			return dataContext
				.Tournaments
				.Include(t => t.Players)
				.Include(t => t.Rounds)
				.Include("Rounds.Matches")
				.Include("Rounds.Matches.Games")
				.FirstOrDefault(t => t.Key == tournamentKey);
		}

		public static Round GetActiveRound(this Tournament tournament)
		{
			if(tournament == null)
				return null;

			if(tournament.ActiveRoundNumber == null)
				return null;

			return tournament
				.Rounds
				.Where(r => r.Number == tournament.ActiveRoundNumber)
				.FirstOrDefault();
		}

		public static Round GetRound(this Tournament tournament, int roundNumber)
		{
			if(tournament == null)
				return null;

			return tournament
				.Rounds
				.Where(r => r.Number == roundNumber)
				.FirstOrDefault();
		}

		public static IEnumerable<Match> GetPlayerMatches(this Tournament tournament, Player player, int? roundNumber = null)
		{
			if(tournament == null)
				return null;

			if(player == null)
				return null;

			return tournament
				.Rounds
				.Where(round => roundNumber == null || round.Number <= roundNumber)
				.SelectMany(round => round.Matches)
				.Where(match => match.Players.Contains(player));
		}

		public static IEnumerable<Game> GetPlayerGames(this Tournament tournament, Player player, int? roundNumber = null)
		{
			if(tournament == null)
				return null;

			if(player == null)
				return null;

			return tournament
				.GetPlayerMatches(player, roundNumber)
				.SelectMany(match => match.Games);
		}

		public static IEnumerable<Player> GetPlayerOpponents(this Tournament tournament, Player player, int? roundNumber = null)
		{
			if(tournament == null)
				return null;

			if(player == null)
				return null;

			return tournament
				.GetPlayerMatches(player, roundNumber)
				.SelectMany(match => match.Players)
				.Except(new[] { player })
				.Distinct();
		}

		public static Match GetMatch(this Round round, int matchNumber)
		{
			if(round == null)
				return null;

			return round
				.Matches
				.Where(m => m.Number == matchNumber)
				.FirstOrDefault();
		}

		public static Match GetMatch(this Round round, Player player)
		{
			if(round == null)
				return null;

			if(player == null)
				return null;

			return round
				.Matches
				.Where(m => m.Players.Contains(player))
				.FirstOrDefault();
		}

		public static Player GetPlayer(this Tournament tournament, string playerName)
		{
			if(tournament == null)
				return null;

			return tournament
				.Players
				.Where(p => p.Name == playerName)
				.FirstOrDefault();
		}
	}
}
