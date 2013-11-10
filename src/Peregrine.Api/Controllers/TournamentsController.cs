using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Api.Services;

namespace Peregrine.Api.Controllers
{
	[RoutePrefix("tournaments")]
	public class TournamentsController : ApiController
	{
		readonly TournamentRenderer TournamentRenderer;
		readonly ActionLinkBuilder ActionLinkBuilder;
		readonly ActionLinkRenderer ActionLinkRenderer;

		public TournamentsController()
		{
			TournamentRenderer = new TournamentRenderer();
			ActionLinkBuilder = new ActionLinkBuilder();
			ActionLinkRenderer = new ActionLinkRenderer();
		}

		[Route(Name = "list-tournaments")]
		public IHttpActionResult Get()
		{
			using(var dataContext = new DataContext())
			{
				var tournaments = dataContext
					.Tournaments
					.ToArray();					// There's got to be a way to not serialize the entire table.

				return Ok(new
				{
					tournaments = tournaments
						.Select(t => TournamentRenderer.RenderSummary(t, Url)),
					_actions = ActionLinkBuilder
						.BuildActions(ControllerContext)
						.Select(al => ActionLinkRenderer.Render(al))
				});
			}
		}

		[Route(Name = "create-tournament")]
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
					"get-tournament",
					new { key = tournament.Key },
					new
					{
						tournament = TournamentRenderer.RenderSummary(tournament, Url),
						_actions = ActionLinkBuilder
							.BuildActions(ControllerContext)
							.Select(al => ActionLinkRenderer.Render(al))
					}
				);
			}
		}
	}
}