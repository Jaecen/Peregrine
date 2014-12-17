using System;
using Peregrine.Data;

namespace Peregrine.Web.Models
{
	public class ActiveRoundRequest
	{
		public int roundNumber;
	}

	public class ActiveRoundResponseProvider
	{
		public ActiveRoundResponse Create(Tournament tournament, int? currentRoundNumber)
		{
			return new ActiveRoundResponse(tournament.Key, currentRoundNumber);
		}
	}

	public class ActiveRoundResponse
	{
		public readonly Guid tournamentKey;
		public readonly int? roundNumber;

		public ActiveRoundResponse(Guid tournamentKey, int? roundNumber)
		{
			this.tournamentKey = tournamentKey;
			this.roundNumber = roundNumber;
		}
	}
}