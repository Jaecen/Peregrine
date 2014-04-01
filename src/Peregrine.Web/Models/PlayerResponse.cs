
namespace Peregrine.Web.Models
{
	public class PlayerResponse
	{
		public readonly string name;
		public readonly bool dropped;

		public PlayerResponse(string name, bool dropped)
		{
			this.name = name;
			this.dropped = dropped;
		}
	}
}