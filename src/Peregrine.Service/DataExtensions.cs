using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Peregrine.Data;

namespace Peregrine.Service
{
	public static class DataExtensions
	{
		public static bool HasStarted(this Tournament tournament)
		{ 
			if(tournament == null)
				return false;

			return tournament.Rounds.Any(round => round.Matches.Any(match => match.Games.Any()));
		}
	}
}