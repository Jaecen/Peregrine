using Peregrine.Data;

namespace Peregrine.Web.Models
{
	public class ActiveRoundResponseProvider
	{
		public ActiveRoundResponse Create(Tournament tournament, int? currentRoundNumber)
		{
			return new ActiveRoundResponse(tournament.Key, currentRoundNumber);
		}
	}
}