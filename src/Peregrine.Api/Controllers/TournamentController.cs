using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Api.Controllers
{
	[RoutePrefix("tournaments/{tournamentKey}")]
	public class TournamentController : ApiController
	{
		[Route(Name = "tournament-get")]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				return Ok(new
					{
						tournament = new
						{
							key = tournament.Key,
							players = tournament
								.Players
								.Select(player => player.Name),
							rounds = tournament
								.Rounds
								.Select(round => round.Number),
						}
					});
			}
		}

		[Route]
		public IHttpActionResult Delete(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				return StatusCode(HttpStatusCode.NoContent);
			}
		}
	}
}
