using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;
using System.Security.Claims;

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
			var userName = User.Identity.Name;
			if(string.IsNullOrEmpty(userName))
				return Ok();

			var userIsAdmin = User.IsInRole("Admin");

			using(var dataContext = new DataContext())
			{
				var tournaments = dataContext
					.Tournaments
					.Where(t => userIsAdmin
						|| t
						.Organizers
						.Where(o => o.UserName == userName)
						.FirstOrDefault() != null)
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
				var userName = User.Identity.Name;
				var user = dataContext
					.Users
					.Where(u => u.UserName == userName)
					.FirstOrDefault();

				var tournament = dataContext
					.Tournaments
					.Add(new Tournament
					{
						Key = Guid.NewGuid(),
						Name = details.name,
						Seed = rng.Next(),
						Players = new List<Player>(),
						Organizers = new[] { user },
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