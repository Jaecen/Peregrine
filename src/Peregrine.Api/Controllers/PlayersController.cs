using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Api.Services;

namespace Peregrine.Api.Controllers
{
	[RoutePrefix("tournament/{key}")]
	public class PlayersController : ApiController
	{
		readonly PlayerRenderer PlayerRenderer;
		readonly ActionLinkBuilder ActionLinkBuilder;
		readonly ActionLinkRenderer ActionLinkRenderer;

		public PlayersController()
		{
			PlayerRenderer = new PlayerRenderer();
			ActionLinkBuilder = new ActionLinkBuilder();
			ActionLinkRenderer = new ActionLinkRenderer();
		}

		[Route("players", Name = "list-players")]
		public IHttpActionResult Get(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				return Ok(new
				{
					players = tournament.Players
						.Select(player => PlayerRenderer.RenderSummary(tournament, player, Url)),
					_actions = ActionLinkBuilder
						.BuildActions(ControllerContext)
						.Select(al => ActionLinkRenderer.Render(al)),
				});
			}
		}

		[Route("player/{name}", Name = "add-player")]
		public IHttpActionResult Put(Guid key, string name)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				// Can't add players after first round has started
				if(tournament.HasStarted())
					return BadRequest("Can not add players once a game result has been recorded.");

				// Can't add a player with the same name as an existing one
				if(tournament
					.Players
					.Where(p => p.Name == name)
					.Any())
					return Conflict();

				var player = new Player
				{
					Name = name,
				};

				tournament.Players.Add(player);
				dataContext.SaveChanges();

				return CreatedAtRoute(
					"get-player",
					new { key = tournament.Key, name = player.Name },
					new
					{
						player = PlayerRenderer.RenderSummary(tournament, player, Url),
						_actions = ActionLinkBuilder
							.BuildActions(ControllerContext)
							.Select(al => ActionLinkRenderer.Render(al))
					}
				);
			}
		}
	}
}
