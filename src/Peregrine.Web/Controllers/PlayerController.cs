using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/players/{playerName}")]
	public class PlayerController : ApiController
	{
		[Route]
		public IHttpActionResult Get(Guid tournamentKey, string playerName)
		{
			using(var dataContext = new DataContext())
			{
				var player = dataContext
					.GetTournament(tournamentKey)
					.GetPlayer(playerName);

				if(player == null)
					return NotFound();

				return Ok(Render(player));
			}
		}

		[Route]
		public IHttpActionResult Put(Guid tournamentKey, string playerName)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var tournamentHasResults = tournament
					.Rounds
					.Where(round => round
						.Matches
						.Any()
					)
					.Any();

				if(tournamentHasResults)
					return StatusCode(System.Net.HttpStatusCode.MethodNotAllowed);

				var player = tournament
					.GetPlayer(playerName);

				if(player != null)
					return Conflict();

				player = dataContext
					.Players
					.Add(new Player
					{
						Name = playerName,
					});

				tournament.Players.Add(player);

				dataContext.SaveChanges();

				return Ok(Render(player));
			}
		}

		[Route]
		public IHttpActionResult Delete(Guid tournamentKey, string playerName)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var player = tournament
					.GetPlayer(playerName);

				if(player == null)
					return NotFound();

				var tournamentHasResults = tournament
					.Rounds
					.Where(round => round
						.Matches
						.Any()
					)
					.Any();

				if(tournamentHasResults)
				{
					// Drop
					player.Dropped = true;
					dataContext.SaveChanges();

					return Ok(Render(player));
				}
				else
				{
					// Delete
					dataContext.Players.Remove(player);
					dataContext.SaveChanges();

					return ResponseMessage(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NoContent)
						{
							Content = new StringContent(String.Empty, Encoding.UTF8, "application/json")
						});
				}
			}
		}

		object Render(Player player)
		{
			return new
			{
				name = player.Name,
				dropped = player.Dropped,
			};
		}
	}
}