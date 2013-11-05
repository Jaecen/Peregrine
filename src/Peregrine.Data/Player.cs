using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Peregrine.Data
{
	public class Player
	{
		[JsonIgnore] public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
