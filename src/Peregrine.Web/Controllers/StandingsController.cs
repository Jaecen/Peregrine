using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/standings")]
	public class StandingsController : ApiController
	{
		readonly StatsProvider StatsManager;

		public StandingsController()
		{
			StatsManager = new StatsProvider();
		}

		[Route]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var standings = tournament
					.Players
					.Select(player => new
					{
						name = player.Name,
						matchPoints = StatsManager.GetMatchPoints(tournament, player),
						matchWinPercentage = StatsManager.GetMatchWinPercentage(tournament, player),
						opponentsMatchWinPercentage = StatsManager.GetOpponentsMatchWinPercentage(tournament, player),
						gamePoints = StatsManager.GetGamePoints(tournament, player),
						gameWinPercentage = StatsManager.GetGameWinPercentage(tournament, player),
						opponentsGameWinPercentage = StatsManager.GetOpponentsGameWinPercentage(tournament, player),
					})
					.OrderByDescending(o => o.matchPoints)
					.ThenByDescending(o => o.opponentsMatchWinPercentage)
					.ThenByDescending(o => o.gameWinPercentage)
					.ThenByDescending(o => o.opponentsGameWinPercentage)
					.Select((o, rank) => new
					{
						rank,
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