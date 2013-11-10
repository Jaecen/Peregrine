using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Api.Services;

namespace Peregrine.Api.Controllers
{
	[RoutePrefix("tournament/{key}/round/{roundNumber}")]
	public class RoundController : ApiController
	{
		readonly RoundManager RoundManager;
		readonly RoundRenderer RoundRenderer;
		readonly ActionLinkBuilder ActionLinkBuilder;
		readonly ActionLinkRenderer ActionLinkRenderer;

		public RoundController()
		{
			RoundManager = new RoundManager();
			RoundRenderer = new RoundRenderer();
			ActionLinkBuilder = new ActionLinkBuilder();
			ActionLinkRenderer = new ActionLinkRenderer();
		}

		[Route(Name = "get-round")]
		public IHttpActionResult Get(Guid key, int roundNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				var round = tournament.GetRound(roundNumber);
				if(round == null)
					if(roundNumber == 1)
						round = RoundManager.GenerateFirstRound(tournament);
					else
						return NotFound();

				return Ok(new
				{
					rounds = RoundRenderer.RenderSummary(tournament, round, Url),
					_actions = ActionLinkBuilder
						.BuildActions(ControllerContext)
						.Select(al => ActionLinkRenderer.Render(al)),
				});
				
			}
		}
	}
}
