using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Api.Services;

namespace Peregrine.Api.Controllers
{
	[RoutePrefix("tournament/{key}/round/{roundNumber}/match/{matchNumber}")]
	public class MatchController : ApiController
    {
		public enum MatchResult
		{
			Win,
			Draw,
		};

		readonly MatchRenderer MatchRenderer;
		readonly ActionLinkBuilder ActionLinkBuilder;
		readonly ActionLinkRenderer ActionLinkRenderer;

		public MatchController()
		{
			MatchRenderer = new MatchRenderer();
			ActionLinkBuilder = new ActionLinkBuilder();
			ActionLinkRenderer = new ActionLinkRenderer();
		}

		[Route(Name = "get-match")]
		public IHttpActionResult Get(Guid key, int roundNumber, int matchNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				var round = tournament.GetRound(roundNumber);
				var match = round.GetMatch(matchNumber);

				if(match == null)
					return NotFound();

				return Ok(new
				{
					match = MatchRenderer.RenderSummary(tournament, round, match, Url),
					_actions = ActionLinkBuilder
						.BuildActions(ControllerContext)
						.Select(al => ActionLinkRenderer.Render(al)),
				});
			}
		}

		[Route(Name = "add-result")]
		public IHttpActionResult Put(Guid key, int roundNumber, string playerName, MatchResult result)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				var player = tournament.GetPlayer(playerName);
				var round = tournament.GetRound(roundNumber);
				var match = round.GetMatch(player);

				if(match == null)
					return NotFound();

				// Null winner indicates draw
				match.Games.Add(new Game
				{
					Number = match.Games.Count + 1,
					Winner = result == MatchResult.Win ? player : null,
				});

				dataContext.SaveChanges();

				return Ok(new
				{
					match = MatchRenderer.RenderSummary(tournament, round, match, Url),
					_actions = ActionLinkBuilder
						.BuildActions(ControllerContext)
						.Select(al => ActionLinkRenderer.Render(al)),
				});
			}
		}
    }
}
