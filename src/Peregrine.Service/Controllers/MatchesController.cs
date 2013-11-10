using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Service.Services;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/round/{roundNumber}/matches")]
	public class MatchesController : ApiController
	{
		readonly MatchRenderer MatchRenderer;
		readonly ActionLinkBuilder ActionLinkBuilder;
		readonly ActionLinkRenderer ActionLinkRenderer;

		public MatchesController()
		{
			MatchRenderer = new MatchRenderer();
			ActionLinkBuilder = new ActionLinkBuilder();
			ActionLinkRenderer = new ActionLinkRenderer();
		}

		[Route(Name = "list-matches")]
		public IHttpActionResult GetList(Guid key, int roundNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				var round = tournament.GetRound(roundNumber);

				if(round == null)
					return NotFound();

				return Ok(new
				{
					matches = round.Matches
						.Select(match => MatchRenderer.RenderSummary(tournament, round, match, Url)),
					_actions = ActionLinkBuilder
						.BuildActions(ControllerContext)
						.Select(al => ActionLinkRenderer.Render(al)),
				});
			}
		}
	}
}
