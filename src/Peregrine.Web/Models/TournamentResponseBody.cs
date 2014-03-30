using System;
using System.Linq;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Models
{
	public class TournamentResponseBody
	{
		public readonly Guid key;
		public readonly string name;
		public readonly bool started;
		public readonly bool finished;

		public TournamentResponseBody(Guid key, string name, bool started, bool finished)
		{
			this.key = key;
			this.name = name;
			this.started = started;
			this.finished = finished;
		}
	}
}