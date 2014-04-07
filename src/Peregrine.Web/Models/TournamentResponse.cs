using System;

namespace Peregrine.Web.Models
{
	public class TournamentResponse
	{
		public readonly Guid key;
		public readonly string name;
		public readonly bool started;
		public readonly bool finished;
		public readonly int totalRounds;
		public readonly int? activeRoundNumber;

		public TournamentResponse(Guid key, string name, bool started, bool finished, int totalRounds, int? activeRoundNumber)
		{
			this.key = key;
			this.name = name;
			this.started = started;
			this.finished = finished;
			this.totalRounds = totalRounds;
			this.activeRoundNumber = activeRoundNumber;
		}
	}
}