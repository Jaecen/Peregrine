﻿using System;
using System.Collections.Generic;

namespace Peregrine.Data
{
	public class Tournament
	{
		public virtual int Id { get; set; }
		public virtual Guid Key { get; set; }
		public virtual ICollection<Player> Players { get; set; }
		public virtual ICollection<Round> Rounds { get; set; }
	}
}