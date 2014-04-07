using System;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Models
{
	public class TournamentResponseProvider
	{
		readonly TournamentManager TournamentManager;

		public TournamentResponseProvider(TournamentManager tournamentManager)
		{
			if(tournamentManager == null)
				throw new ArgumentNullException("tournamentManager");

			TournamentManager = tournamentManager;
		}

		public TournamentResponse Create(Tournament tournament)
		{
			if(tournament == null)
				throw new ArgumentNullException("tournament");

			var tournamentState = TournamentManager.GetTournamentState(tournament);

			return new TournamentResponse(
				key: tournament.Key,
				name: tournament.Name,
				started: tournamentState >= TournamentState.Started,
				finished: tournamentState >= TournamentState.Finished,
				totalRounds: TournamentManager.GetTotalRounds(tournament),
				activeRoundNumber: tournament.ActiveRoundNumber
			);
		}
	}
}