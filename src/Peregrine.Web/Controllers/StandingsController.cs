using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	public class StandingsController : ApiController
	{
		readonly StatsProvider StatsProvider;

		public StandingsController()
		{
			StatsProvider = new StatsProvider();
		}

		[Route("api/tournaments/{tournamentKey}/standings")]
		[Route("api/tournaments/{tournamentKey}/round/{roundNumber:min(1)}/standings")]
		public IHttpActionResult Get(Guid tournamentKey, int? roundNumber = null)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				if(roundNumber.HasValue && tournament.GetRound(roundNumber.Value) == null)
					return NotFound();

				var standings = tournament
					.Players
					.Select(player => new
					{
						name = player.Name,
						matchPoints = StatsProvider.GetMatchPoints(tournament, player, roundNumber),
						matchWinPercentage = StatsProvider.GetMatchWinPercentage(tournament, player, roundNumber),
						opponentsMatchWinPercentage = StatsProvider.GetOpponentsMatchWinPercentage(tournament, player, roundNumber),
						gamePoints = StatsProvider.GetGamePoints(tournament, player, roundNumber),
						gameWinPercentage = StatsProvider.GetGameWinPercentage(tournament, player, roundNumber),
						opponentsGameWinPercentage = StatsProvider.GetOpponentsGameWinPercentage(tournament, player, roundNumber),
					})
					.OrderByDescending(o => o.matchPoints)
					.ThenByDescending(o => o.opponentsMatchWinPercentage)
					.ThenByDescending(o => o.gameWinPercentage)
					.ThenByDescending(o => o.opponentsGameWinPercentage)
					.Select((o, rank) => new
					{
						rank = rank + 1,
						o.name,
						o.matchPoints,
						o.matchWinPercentage,
						o.opponentsMatchWinPercentage,
						o.gamePoints,
						o.gameWinPercentage,
						o.opponentsGameWinPercentage,
					})
					.ToArray();

				return Ok(standings);
			}
		}
	}
}