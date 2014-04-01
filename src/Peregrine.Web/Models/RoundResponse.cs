using System.Collections.Generic;
using System.Linq;

namespace Peregrine.Web.Models
{
	public class RoundResponse
	{
		public readonly int number;
		public readonly bool started;
		public readonly bool completed;
		public readonly bool final;
		public readonly IEnumerable<MatchResponse> matches;

		public RoundResponse(int number, bool started, bool completed, bool final, IEnumerable<MatchResponse> matches)
		{
			this.number = number;
			this.started = started;
			this.completed = completed;
			this.final = final;
			this.matches = matches ?? Enumerable.Empty<MatchResponse>();
		}
	}
}