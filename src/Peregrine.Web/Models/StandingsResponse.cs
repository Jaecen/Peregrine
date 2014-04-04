
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Peregrine.Web.Models
{
	public class StandingsResponse
	{
		public readonly decimal minMatchPoints;
		public readonly decimal maxMatchPoints;
		public readonly decimal minMatchWinPercentage;
		public readonly decimal maxMatchWinPercentage;
		public readonly decimal minOpponentsMatchWinPercentage;
		public readonly decimal maxOpponentsMatchWinPercentage;
		public readonly decimal minGamePoints;
		public readonly decimal maxGamePoints;
		public readonly decimal minGameWinPercentage;
		public readonly decimal maxGameWinPercentage;
		public readonly decimal minOpponentsGameWinPercentage;
		public readonly decimal maxOpponentsGameWinPercentage;
		public readonly IEnumerable<PlayerStandingResponse> playerStandings;

		public StandingsResponse(IEnumerable<PlayerStandingResponse> playerStandings)
		{
			playerStandings = playerStandings ?? Enumerable.Empty<PlayerStandingResponse>();

			var matchPoints = playerStandings.Select(standing => standing.matchPoints);
			this.minMatchPoints = matchPoints.Min();
			this.maxMatchPoints = matchPoints.Max();

			var matchWinPercentage = playerStandings.Select(standing => standing.matchWinPercentage);
			this.minMatchWinPercentage = matchWinPercentage.Min();
			this.maxMatchWinPercentage = matchWinPercentage.Max();

			var opponentsMatchWinPercentage = playerStandings.Select(standing => standing.opponentsMatchWinPercentage);
			this.minOpponentsMatchWinPercentage = opponentsMatchWinPercentage.Min();
			this.maxOpponentsMatchWinPercentage = opponentsMatchWinPercentage.Max();

			var gamePoints = playerStandings.Select(standing => standing.gamePoints);
			this.minGamePoints = gamePoints.Min();
			this.maxGamePoints = gamePoints.Max();

			var gameWinPercentage = playerStandings.Select(standing => standing.gameWinPercentage);
			this.minGameWinPercentage = gameWinPercentage.Min();
			this.maxGameWinPercentage = gameWinPercentage.Max();

			var opponentsGameWinPercentage = playerStandings.Select(standing => standing.opponentsGameWinPercentage);
			this.minOpponentsGameWinPercentage = opponentsGameWinPercentage.Min();
			this.maxOpponentsGameWinPercentage = opponentsGameWinPercentage.Max();

			this.playerStandings = playerStandings
				.Select(standing => new PlayerStandingResponse(
					source: standing,
					minMatchPoints: this.minMatchPoints,
					maxMatchPoints: this.maxMatchPoints,
					minMatchWinPercentage: this.minMatchWinPercentage,
					maxMatchWinPercentage: this.maxMatchWinPercentage,
					minOpponentsMatchWinPercentage: this.minOpponentsMatchWinPercentage,
					maxOpponentsMatchWinPercentage: this.maxOpponentsMatchWinPercentage,
					minGamePoints: this.minGamePoints,
					maxGamePoints: this.maxGamePoints,
					minGameWinPercentage: this.minGameWinPercentage,
					maxGameWinPercentage: this.maxGameWinPercentage,
					minOpponentsGameWinPercentage: this.minOpponentsGameWinPercentage,
					maxOpponentsGameWinPercentage: this.maxOpponentsGameWinPercentage
				))
				.ToArray();
		}
	}
}