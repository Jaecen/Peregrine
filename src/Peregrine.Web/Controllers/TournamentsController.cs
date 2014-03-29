using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments")]
	public class TournamentsController : ApiController
	{
		[Route]
		public IHttpActionResult Get()
		{
			using(var dataContext = new DataContext())
			{
				var tournamentKeys = dataContext
					.Tournaments
					.ToArray()
					.Select(tournament => new TournamentResponseBody(tournament));

				return Ok(tournamentKeys);
			}
		}

		[Route]
		public IHttpActionResult Post([FromBody]TournamentRequestBody requestBody)
		{
			var rng = new Random();
			var details = requestBody ?? new TournamentRequestBody();

			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.Add(new Tournament
					{
						Key = Guid.NewGuid(),
						Name = details.name,
						Seed = rng.Next(),
					});

				dataContext.SaveChanges();

				return CreatedAtRoute(
					"tournament-get",
					new { tournamentKey = tournament.Key },
					new TournamentResponseBody(tournament)
				);
			}
		}
	}
}