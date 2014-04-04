
namespace Peregrine.Web.Models
{
	public class PlayerStandingResponse
	{
		public readonly int rank;
		public readonly string playerName;
		public readonly decimal matchPoints;
		public readonly decimal relativeMatchPoints;
		public readonly decimal absoluteMatchPoints;
		public readonly decimal matchWinPercentage;
		public readonly decimal relativeMatchWinPercentage;
		public readonly decimal absoluteMatchWinPercentage;
		public readonly decimal opponentsMatchWinPercentage;
		public readonly decimal relativeOpponentsMatchWinPercentage;
		public readonly decimal absoluteOpponentsMatchWinPercentage;
		public readonly decimal gamePoints;
		public readonly decimal relativeGamePoints;
		public readonly decimal absoluteGamePoints;
		public readonly decimal gameWinPercentage;
		public readonly decimal relativeGameWinPercentage;
		public readonly decimal absoluteGameWinPercentage;
		public readonly decimal opponentsGameWinPercentage;
		public readonly decimal relativeOpponentsGameWinPercentage;
		public readonly decimal absoluteOpponentsGameWinPercentage;

		public PlayerStandingResponse(int rank, string playerName, decimal matchPoints, decimal matchWinPercentage, decimal opponentsMatchWinPercentage, decimal gamePoints, decimal gameWinPercentage, decimal opponentsGameWinPercentage)
		{
			this.rank = rank;
			this.playerName = playerName;
			this.matchPoints = matchPoints;
			this.matchWinPercentage = matchWinPercentage;
			this.opponentsMatchWinPercentage = opponentsMatchWinPercentage;
			this.gamePoints = gamePoints;
			this.gameWinPercentage = gameWinPercentage;
			this.opponentsGameWinPercentage = opponentsGameWinPercentage;
		}

		public PlayerStandingResponse(
			PlayerStandingResponse source,
			decimal minMatchPoints,
			decimal maxMatchPoints,
			decimal minMatchWinPercentage,
			decimal maxMatchWinPercentage,
			decimal minOpponentsMatchWinPercentage,
			decimal maxOpponentsMatchWinPercentage,
			decimal minGamePoints,
			decimal maxGamePoints,
			decimal minGameWinPercentage,
			decimal maxGameWinPercentage,
			decimal minOpponentsGameWinPercentage,
			decimal maxOpponentsGameWinPercentage)
		{
			this.rank = source.rank;
			this.playerName = source.playerName;
			this.matchPoints = source.matchPoints;
			this.relativeMatchPoints = (source.matchPoints - minMatchPoints) / (maxMatchPoints - minMatchPoints);
			this.absoluteMatchPoints = source.matchPoints / maxMatchPoints;
			this.matchWinPercentage = source.matchWinPercentage;
			this.relativeMatchWinPercentage = (source.matchWinPercentage - minMatchWinPercentage) / (maxMatchWinPercentage - minMatchWinPercentage);
			this.absoluteMatchWinPercentage = source.matchWinPercentage / maxMatchWinPercentage;
			this.opponentsMatchWinPercentage = source.opponentsMatchWinPercentage;
			this.relativeOpponentsMatchWinPercentage = (source.opponentsMatchWinPercentage - minOpponentsMatchWinPercentage) / (maxOpponentsMatchWinPercentage - minOpponentsMatchWinPercentage);
			this.absoluteOpponentsMatchWinPercentage = source.opponentsMatchWinPercentage / maxOpponentsMatchWinPercentage;
			this.gamePoints = source.gamePoints;
			this.relativeGamePoints = (source.gamePoints - minGamePoints) / (maxGamePoints - minGamePoints);
			this.absoluteGamePoints = source.gamePoints / maxGamePoints;
			this.gameWinPercentage = source.gameWinPercentage;
			this.relativeGameWinPercentage = (source.gameWinPercentage - minGameWinPercentage) / (maxGameWinPercentage - minGameWinPercentage);
			this.absoluteGameWinPercentage = source.gameWinPercentage / maxGameWinPercentage;
			this.opponentsGameWinPercentage = source.opponentsGameWinPercentage;
			this.relativeOpponentsGameWinPercentage = (source.opponentsGameWinPercentage - minOpponentsGameWinPercentage) / (maxOpponentsGameWinPercentage - minOpponentsGameWinPercentage);
			this.absoluteOpponentsGameWinPercentage = source.opponentsGameWinPercentage / maxOpponentsGameWinPercentage;
		}
	}
}