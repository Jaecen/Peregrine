using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Api.Services;

namespace Peregrine.Api.Controllers
{
	[RoutePrefix("tournament/{key}/rounds")]
	public class RoundsController : ApiController
	{
		readonly RoundManager RoundManager;
		readonly RoundRenderer RoundRenderer;
		readonly ActionLinkBuilder ActionLinkBuilder;
		readonly ActionLinkRenderer ActionLinkRenderer;

		public RoundsController()
		{
			RoundManager = new RoundManager();
			RoundRenderer = new RoundRenderer();
			ActionLinkBuilder = new ActionLinkBuilder();
			ActionLinkRenderer = new ActionLinkRenderer();
		}

		[Route(Name = "list-rounds")]
		public IHttpActionResult GetList(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				return Ok(new
				{
					rounds = tournament
						.Rounds
						.Select(round => RoundRenderer.RenderSummary(tournament, round, Url)),
					_actions = ActionLinkBuilder
						.BuildActions(ControllerContext)
						.Select(al => ActionLinkRenderer.Render(al)),
				});
			}
		}
	}
}
