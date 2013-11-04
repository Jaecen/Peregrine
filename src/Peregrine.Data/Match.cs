using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Peregrine.Data
{
	public class Match
	{
		public virtual int Id { get; set; }
		public virtual ICollection<Player> Players { get; set; }
		public virtual ICollection<Game> Games { get; set; }
	}
}