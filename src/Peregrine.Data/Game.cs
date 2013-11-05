using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Peregrine.Data
{
	public class Game
	{
		[JsonIgnore] public virtual int Id { get; set; }
		public virtual int Ordinal { get; set; }
		public Player Winner { get; set; }
	}
}
