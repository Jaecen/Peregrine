using Peregrine.Data;

namespace Peregrine.Web.Models
{
	public class PlayerResponseProvider
	{
		public PlayerResponse Create(Player player)
		{
			return new PlayerResponse(
				name: player.Name,
				dropped: player.Dropped
			);
		}
	}
}