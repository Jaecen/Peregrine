using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Peregrine.Data;

namespace Peregrine.Web.Services
{
	public enum TournamentState
	{
		None = 0,
		Started = 1,
		Finished = 2
	}

	public class TournamentManager
	{
		readonly RoundManager RoundManager;

		public TournamentManager(RoundManager roundManager)
		{
			if(roundManager == null)
				throw new ArgumentNullException("roundManager");

			RoundManager = roundManager;
		}

		public TournamentState GetTournamentState(Tournament tournament)
		{
			if(tournament == null)
				throw new ArgumentNullException("tournament");

			var firstRoundState = RoundManager.GetRoundState(tournament, 1);

			if(firstRoundState < RoundState.Committed)
				return TournamentState.None;

			var lastRound = RoundManager.GetMaxRoundsForTournament(tournament);
			var lastRoundState = RoundManager.GetRoundState(tournament, lastRound);

			if(lastRoundState < RoundState.Completed)
				return TournamentState.Started;

			return TournamentState.Finished;
		}

		public int? GetCurrentRoundNumber(Tournament tournament)
		{
			if(tournament == null)
				throw new ArgumentNullException("tournament");

			var roundStatus = Enumerable
				.Range(1, GetTotalRounds(tournament))
				.Select(roundNumber => new
				{
					Number = roundNumber,
					State = RoundManager.GetRoundState(tournament, roundNumber)
				});

			if(!roundStatus.Any())
				return null;

			var currentRound = roundStatus
				.SkipWhile(o => o.State >= RoundState.Final)
				.FirstOrDefault();

			if(currentRound == null || currentRound.State == RoundState.Invalid)
				return null;

			return currentRound.Number;
		}

		public int GetTotalRounds(Tournament tournament)
		{
			if(tournament == null)
				throw new ArgumentNullException("tournament");

			return RoundManager.GetMaxRoundsForTournament(tournament);
		}
	}
}