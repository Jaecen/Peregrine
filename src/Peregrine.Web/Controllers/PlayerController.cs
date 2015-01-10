using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/players/{playerName}")]
	public class PlayerController : ApiController
	{
		readonly PlayerResponseProvider PlayerResponseProvider;

		public PlayerController(PlayerResponseProvider playerResponseProvider)
		{
			if(playerResponseProvider == null)
				throw new ArgumentNullException("playerResponseProvider");

			PlayerResponseProvider = playerResponseProvider;
		}

		[Route(Name="player-get")]
		public IHttpActionResult Get(Guid tournamentKey, string playerName)
		{
			using(var dataContext = new DataContext())
			{
				var player = dataContext
					.GetTournament(tournamentKey)
					.GetPlayer(playerName);

				if(player == null)
					return NotFound();

				return Ok(PlayerResponseProvider.Create(player));
			}
		}

		[Route]
		[Authorize]
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
						Dropped = false,
					});

				tournament.Players.Add(player);

				dataContext.SaveChanges();

				return CreatedAtRoute(
						"player-get",
						new { tournamentKey = tournament.Key, playerName = player.Name },
						PlayerResponseProvider.Create(player)
					);
			}
		}

		[Route]
		[Authorize]
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

					return Ok(PlayerResponseProvider.Create(player));
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
	}
}