using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Service.Services;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/player/{name}")]
	public class PlayerController : ApiController
	{
		readonly PlayerRenderer PlayerRenderer;
		readonly ActionLinkBuilder ActionLinkBuilder;
		readonly ActionLinkRenderer ActionLinkRenderer;

		public PlayerController()
		{
			PlayerRenderer = new PlayerRenderer();
			ActionLinkBuilder = new ActionLinkBuilder();
			ActionLinkRenderer = new ActionLinkRenderer();
		}

		[Route(Name = "get-player")]
		public IHttpActionResult Get(Guid key, string name)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				var player = tournament.GetPlayer(name);

				if(player == null)
					return NotFound();

				return Ok(new
				{
					player = PlayerRenderer.RenderSummary(tournament, player, Url),
					_actions = ActionLinkBuilder
						.BuildActions(ControllerContext)
						.Select(al => ActionLinkRenderer.Render(al))
				});
			}
		}

		[Route(Name = "drop-player")]
		public IHttpActionResult Delete(Guid key, string name)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				var player = tournament.GetPlayer(name);
				if(player == null)
					return NotFound();

				if(tournament.HasStarted())
				{
					// Drop if any game results have been recorded
					player.Dropped = true;
					dataContext.SaveChanges();

					return Ok(new
					{
						player = PlayerRenderer.RenderSummary(tournament, player, Url),
						_actions = ActionLinkBuilder
							.BuildActions(ControllerContext)
							.Where(al => al.Name != "drop-player")			// Can't drop a dropped player
							.Select(al => ActionLinkRenderer.Render(al))
					});
				}
				else
				{
					// Delete if not results recorded
					tournament.Players.Remove(player);
					dataContext.SaveChanges();
					return StatusCode(System.Net.HttpStatusCode.NoContent);
				}
			}
		}
	}
}
