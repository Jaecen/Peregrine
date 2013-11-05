using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Peregrine.Data
{
	public class Match
	{
		[JsonIgnore] public virtual int Id { get; set; }
		public virtual ICollection<Player> Players { get; set; }
		public virtual ICollection<Game> Games { get; set; }
	}
}