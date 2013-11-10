using System.Collections.Generic;

namespace Peregrine.Data
{
	public class Round
	{
		public virtual int Id { get; set; }
		public virtual int Number { get; set; }
		public virtual ICollection<Match> Matches { get; set; }
	}
}
