
namespace Peregrine.Data
{
	[System.Diagnostics.DebuggerDisplay("Game {Id}")]
	public class Game
	{
		public virtual int Id { get; set; }
		public virtual int Number { get; set; }
		public virtual Player Winner { get; set; }
	}
}
