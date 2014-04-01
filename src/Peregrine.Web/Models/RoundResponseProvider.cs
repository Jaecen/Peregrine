using System;
using System.Linq;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Models
{
	public class RoundResponseProvider
	{
		readonly RoundManager RoundManager;
		readonly MatchResponseProvider MatchResponseProvider;

		public RoundResponseProvider(RoundManager roundManager, MatchResponseProvider matchResponseProvider)
		{
			if(roundManager == null)
				throw new ArgumentNullException("roundManager");

			if(matchResponseProvider == null)
				throw new ArgumentNullException("matchResponseProvider");

			RoundManager = roundManager;
			MatchResponseProvider = matchResponseProvider;
		}

		public RoundResponse Create(Round round, RoundState roundState)
		{
			if(round == null)
				throw new ArgumentNullException("round");

			return new RoundResponse(
				number: round.Number,
				started: roundState >= RoundState.Committed,
				completed: roundState >= RoundState.Completed,
				final: roundState >= RoundState.Final,
				matches: round
					.Matches
					.OrderBy(match => match.Number)
					.Select(match => MatchResponseProvider.Create(match))
			);
		}

		public RoundResponse Create(Tournament tournament, Round round)
		{
			if(round == null)
				throw new ArgumentNullException("round");

			var roundState = RoundManager.GetRoundState(tournament, round.Number);

			return new RoundResponse(
				number: round.Number,
				started: roundState >= RoundState.Committed,
				completed: roundState >= RoundState.Completed,
				final: roundState >= RoundState.Final,
				matches: round
					.Matches
					.OrderBy(match => match.Number)
					.Select(match => MatchResponseProvider.Create(match))
					.ToArray()
			);
		}
	}
}