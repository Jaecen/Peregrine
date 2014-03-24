using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/standings")]
	public class StandingsController : ApiController
	{
		readonly StatsManager StatsManager;

		public StandingsController()
		{
			StatsManager = new StatsManager();
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
					})
					.OrderByDescending(o => o.matchPoints)
					.ToArray();

				return Ok(standings);
			}
		}
	}
}