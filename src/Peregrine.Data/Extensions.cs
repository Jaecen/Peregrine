using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peregrine.Data
{
	public static class Extensions
	{
		public static Tournament GetTournament(this DataContext dataContext, Guid tournamentKey)
		{
			return dataContext
				.Tournaments
				.FirstOrDefault(t => t.Key == tournamentKey);
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
