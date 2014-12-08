using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments")]
	public class TournamentsController : ApiController
	{
		readonly TournamentResponseProvider TournamentResponseProvider;

		public TournamentsController(TournamentResponseProvider tournamentResponseProvider)
		{
			if(tournamentResponseProvider == null)
				throw new ArgumentNullException("tournamentResponseProvider");

			TournamentResponseProvider = tournamentResponseProvider;
		}

		[Route]
		public IHttpActionResult Get()
		{
			using(var dataContext = new DataContext())
			{
				var tournaments = dataContext
					.Tournaments
					.ToArray()
					.Select(tournament => TournamentResponseProvider.Create(tournament))
					.ToArray();

				return Ok(tournaments);
			}
		}

		[Route]
		[Authorize]
		public IHttpActionResult Post([FromBody]TournamentRequest requestBody)
		{
			var rng = new Random();
			var details = requestBody ?? new TournamentRequest();

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
					TournamentResponseProvider.Create(tournament)
				);
			}
		}
	}
}