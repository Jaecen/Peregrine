using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments")]
	public class TournamentsController : ApiController
	{
		readonly TournamentResponseBodyProvider TournamentResponseBodyProvider;

		public TournamentsController()
		{
			TournamentResponseBodyProvider = new TournamentResponseBodyProvider();
		}

		[Route]
		public IHttpActionResult Get()
		{
			using(var dataContext = new DataContext())
			{
				var tournamentKeys = dataContext
					.Tournaments
					.ToArray()
					.Select(tournament => TournamentResponseBodyProvider.CreateResponseBody(tournament))
					.ToArray();

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
						Players = new List<Player>(),
					});

				dataContext.SaveChanges();

				return CreatedAtRoute(
					"tournament-get",
					new { tournamentKey = tournament.Key },
					TournamentResponseBodyProvider.CreateResponseBody(tournament)
				);
			}
		}
	}
}