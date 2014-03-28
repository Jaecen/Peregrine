using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;

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
					.Select(tournament =>
						new
						{
							key = tournament.Key,
						})
					.ToArray();

				return Ok(tournamentKeys);
			}
		}

		[Route]
		public IHttpActionResult Post()
		{
			var rng = new Random();

			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.Add(new Tournament
					{
						Key = Guid.NewGuid(),
						Seed = rng.Next(),
					});

				dataContext.SaveChanges();

				return CreatedAtRoute(
					"tournament-get",
					new { tournamentKey = tournament.Key },
					new
					{
						key = tournament.Key,
					});
			}
		}
	}
}