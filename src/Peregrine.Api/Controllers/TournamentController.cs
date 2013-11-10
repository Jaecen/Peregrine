using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using Peregrine.Data;
using Peregrine.Api.Model;
using Peregrine.Api.Services;
using System.Net;

namespace Peregrine.Api.Controllers
{
	[RoutePrefix("tournament/{key}")]
	public class TournamentController : ApiController
	{
		readonly ActionLinkBuilder ActionLinkBuilder;
		readonly ActionLinkRenderer ActionLinkRenderer;
		readonly TournamentRenderer TournamentRenderer;

		public TournamentController()
		{
			ActionLinkBuilder = new ActionLinkBuilder();
			ActionLinkRenderer = new ActionLinkRenderer();
			TournamentRenderer = new TournamentRenderer();
		}

		[Route(Name = "get-tournament")]
		public IHttpActionResult Get(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				return Ok(new
				{
					tournament = TournamentRenderer.RenderSummary(tournament, Url),
					_actions = ActionLinkBuilder
						.BuildActions(ControllerContext)
						.Select(al => ActionLinkRenderer.Render(al))
				});
			}
		}

		[Route(Name = "delete-tournament")]
		public IHttpActionResult Delete(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				dataContext.Tournaments.Remove(tournament);
				dataContext.SaveChanges();

				return StatusCode(HttpStatusCode.NoContent);
			}
		}
	}
}
