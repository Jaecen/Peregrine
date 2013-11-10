
namespace Peregrine.Data
{
	public class Game
	{
		public virtual int Id { get; set; }
		public virtual int Number { get; set; }
		public Player Winner { get; set; }
	}
}
