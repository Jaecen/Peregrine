using System;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Models
{
	public class TournamentResponseBodyProvider
	{
		readonly RoundManager RoundManager;

		public TournamentResponseBodyProvider()
		{
			RoundManager = new RoundManager();
		}

		public TournamentResponseBody CreateResponseBody(Tournament tournament)
		{
			if(tournament == null)
				throw new ArgumentNullException();

			var firstRoundState = RoundManager.DetermineRoundState(tournament, 1);
			var started = firstRoundState >= RoundState.Committed;
			var lastRound = RoundManager.GetMaxRoundsForTournament(tournament);
			var lastRoundState = RoundManager.DetermineRoundState(tournament, lastRound);
			var finished = lastRoundState >= RoundState.Completed;

			return new TournamentResponseBody(tournament.Key, tournament.Name, started, finished);
		}
	}
}