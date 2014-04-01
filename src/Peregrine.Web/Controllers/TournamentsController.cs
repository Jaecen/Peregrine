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
		readonly EventPublisher EventPublisher;
		readonly TournamentResponseProvider TournamentResponseProvider;

		public TournamentsController(EventPublisher eventPublisher, TournamentResponseProvider tournamentResponseProvider)
		{
			if(eventPublisher == null)
				throw new ArgumentNullException("eventPublisher");

			if(tournamentResponseProvider == null)
				throw new ArgumentNullException("tournamentResponseProvider");

			EventPublisher = eventPublisher;
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

				EventPublisher.Created(tournament);

				return CreatedAtRoute(
					"tournament-get",
					new { tournamentKey = tournament.Key },
					TournamentResponseProvider.Create(tournament)
				);
			}
		}
	}
}