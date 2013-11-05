using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Peregrine.Data
{
	public class Tournament
	{
		[JsonIgnore] public virtual int Id { get; set; }
		public virtual Guid Key { get; set; }
		public virtual ICollection<Player> Players { get; set; }
		public virtual ICollection<Round> Rounds { get; set; }
	}
}
