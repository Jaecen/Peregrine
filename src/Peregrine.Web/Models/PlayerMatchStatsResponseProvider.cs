using System;
using System.Linq;
using Peregrine.Data;

namespace Peregrine.Web.Models
{
	public class PlayerMatchStatsResponseProvider
	{
		public PlayerMatchStatsResponse Create(Player player, Match match)
		{
			if(player == null)
				throw new ArgumentNullException("player");

			if(match == null)
				throw new ArgumentNullException("match");
			
			return new PlayerMatchStatsResponse(
				name: player.Name,
				dropped: player.Dropped,
				wins: match.Games.Where(game => game.Winner == player).Count(),
				losses: match.Games.Where(game => game.Winner != player && game.Winner != null).Count(),
				draws: match.Games.Where(game => game.Winner == null).Count()
			);
		}
	}
}