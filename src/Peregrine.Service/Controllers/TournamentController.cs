using System;
using System.Collections.Generic;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament")]
	public class TournamentController : ApiController
    {
		[Route(Name = "Player.Options")]
		public virtual IHttpActionResult Options()
		{
			return new ResourceActionResult(ControllerContext, Ok());
		}

		[Route(Name="Tournament.Post")]
		public IHttpActionResult Post()
		{
			using(var dataContext = new DataContext())
			{
				var tournament = new Tournament
				{
					Key = Guid.NewGuid(),
				};

				dataContext.Tournaments.Add(tournament);
				dataContext.SaveChanges();
			
				return CreatedAtRoute(
						"Tournament.Get", 
						new { key = tournament.Key }, 
						this.RenderSummary(tournament)
					)
					.AsResource(
						ControllerContext,
						new Dictionary<string, Uri>
						{
							{ "players", new Uri(Url.Link("Players.Get", new { key = tournament.Key })) },
							{ "rounds", new Uri(Url.Link("Rounds.Get", new { key = tournament.Key })) },
						}
					);
			}
		}

		[Route("tournament/{key}", Name = "Tournament.Get")]
		public IHttpActionResult Get(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound()
						.AsResource(
							ControllerContext,
							new Dictionary<string, Uri>
							{
								{ "players", new Uri(Url.Link("Players.Get", new { key = tournament.Key })) },
								{ "rounds", new Uri(Url.Link("Rounds.Get", new { key = tournament.Key })) },
							}
						);


				return Ok(
						this.RenderSummary(tournament)
					)
					.AsResource(
						ControllerContext,
						new Dictionary<string, Uri>
						{
							{ "players", new Uri(Url.Link("Players.Get", new { key = tournament.Key })) },
							{ "rounds", new Uri(Url.Link("Rounds.Get", new { key = tournament.Key })) },
						}
					);
			}
		}
    }
}
