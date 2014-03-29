using System;
using Peregrine.Data;

namespace Peregrine.Web.Models
{
	public class TournamentResponseBody
	{
		public readonly Guid key;
		public readonly string name;

		public TournamentResponseBody(Tournament tournament)
		{
			if(tournament == null)
				throw new ArgumentNullException();

			key = tournament.Key;
			name = tournament.Name;
		}
	}
}