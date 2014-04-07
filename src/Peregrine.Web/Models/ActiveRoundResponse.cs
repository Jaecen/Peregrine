using System;

namespace Peregrine.Web.Models
{
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