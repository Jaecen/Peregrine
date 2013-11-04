using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Peregrine.Data
{
	public class Game
	{
		public virtual int Id { get; set; }
		public virtual int Ordinal { get; set; }
		public Player Winner { get; set; }
	}
}
