
namespace Peregrine.Data
{
	[System.Diagnostics.DebuggerDisplay("Player {Name}")]
	public class Player
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual bool Dropped { get; set; }
	}
}
