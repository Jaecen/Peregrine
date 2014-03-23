using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/rounds/{roundNumber:int:min(1)}/{playerName}")]
	public class MatchResultController : ApiController
	{
		readonly RoundManager RoundManager;

		public MatchResultController()
		{
			RoundManager = new RoundManager();
		}

		[Route("win/{count}")]
		public IHttpActionResult Post(Guid tournamentKey, int roundNumber, string playerName, int count)
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

				// Rounds have five states: projected, committed, and completed, finalized, and invalid
				// - A round is finalized when it's been completed and a result entered for the next round
				// - A round is completed when all matches have the minimum number of results
				// - A round is committed when the first result is submitted. A result can't be submitted until the previous round is completed.
				// - A round is projected when the previous round is completed by no results have been submitted.
				// - A round is invalid if it's more than one greater than the last completed round number
				// When a round becomes committed, it's written to the database and can't be changed. All matches must have a result submitted to move forward.

				var roundState = RoundManager.DetermineRoundState(tournament, roundNumber);
				if(roundState == RoundState.Invalid)
					return NotFound();

				// Can't add results to finalized rounds
				if(roundState == RoundState.Finalized)
					return StatusCode(System.Net.HttpStatusCode.MethodNotAllowed);

				var round = tournament
					.Rounds
					.Where(r => r.Number == roundNumber)
					.First();

				if(roundState == RoundState.Projected)
				{
					// Create and save the matches. State is now committed.
					var matches = RoundManager.CreateMatches(tournament, roundNumber);
					round.Matches = matches;
				}

				// Committed and Completed rounds accept new results.
				// Find the match for this player
				var match = tournament
					.GetRound(roundNumber)
					.GetMatch(player);

				if(match == null)
					return NotFound();

				// Clear all the winning games for this player
				var existingGames = match
					.Games
					.Where(game => game.Winner == player)
					.ToArray();

				foreach(var game in existingGames)
					match.Games.Remove(game);

				// Add new winning games
				for(int wins = 0; wins < count; wins++)
					match
						.Games
						.Add(new Game
							{
								Winner = player,
							});

				dataContext.SaveChanges();

				return Ok(new
					{
						players = match
							.Players
							.Select(p => p.Name)
							.ToArray()
					});
			}
		}

		//[Route("draw/{count}")]
		//public IHttpActionResult Post(Guid tournamentKey, int roundNumber, string playerName, int count)
		//{

		//}
	}
}