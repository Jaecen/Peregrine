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
		readonly MatchRenderer MatchRenderer;
		readonly ActionLinkBuilder ActionLinkBuilder;
		readonly ActionLinkRenderer ActionLinkRenderer;

		public RoundController()
		{
			RoundManager = new RoundManager();
			RoundRenderer = new RoundRenderer();
			MatchRenderer = new MatchRenderer();
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

		public enum MatchResult
		{
			Win,
			Draw,
		};

		[Route("{playerName}/{result}", Name = "add-result")]
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
