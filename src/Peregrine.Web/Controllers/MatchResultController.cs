using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/rounds/{roundNumber:int:min(1)}/{playerName}")]
	public class MatchResultController : ApiController
	{
		readonly RoundManager RoundManager;
		readonly RoundResponseProvider RoundResponseProvider;

		public MatchResultController(RoundManager roundManager, RoundResponseProvider roundResponseProvider)
		{
			if(roundManager == null)
				throw new ArgumentException("roundManager");

			if(roundResponseProvider == null)
				throw new ArgumentException("roundResponseProvider");

			RoundManager = roundManager;
			RoundResponseProvider = roundResponseProvider;
		}

		[Route("{result:regex(^(draws|wins)$)}/{count}")]
		public IHttpActionResult Put(Guid tournamentKey, int roundNumber, string playerName, string result, int count)
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

				Player winningPlayer;
				if(result == "wins")
					winningPlayer = player;
				else if(result == "draws")
					winningPlayer = null;
				else
					return BadRequest("result must be either 'wins' or 'draws'");

				if(roundNumber > RoundManager.GetMaxRoundsForTournament(tournament))
					return NotFound();

				var roundState = RoundManager.GetRoundState(tournament, roundNumber);
				if(roundState == RoundState.Invalid)
					return NotFound();

				// Can't add results to finalized rounds
				if(roundState >= RoundState.Final)
					return StatusCode(System.Net.HttpStatusCode.MethodNotAllowed);

				var round = tournament
					.GetRound(roundNumber)
					?? new Round
					{
						Number = roundNumber,
						Matches = RoundManager.CreateMatches(tournament, roundNumber)
					};

				if(roundState == RoundState.Projected)
				{
					// Add the new round to the tournament.
					tournament.Rounds.Add(round);
				}

				// Committed and Completed rounds accept new results.
				// Find the match for this player
				var match = round
					.GetMatch(player);

				if(match == null)
					return NotFound();

				// Can't change results of byes
				if(match.Players.Count() == 1)
					return StatusCode(System.Net.HttpStatusCode.MethodNotAllowed);

				// Clear all the winning games for this player
				// Null for winner means a draw
				var existingGames = match
					.Games
					.Where(game => game.Winner == winningPlayer)
					.ToArray();

				foreach(var game in existingGames)
					match.Games.Remove(game);

				// Add new winning games
				for(int wins = 0; wins < count; wins++)
					match
						.Games
						.Add(new Game
						{
							Winner = winningPlayer,
						});

				dataContext.SaveChanges();

				return Ok(RoundResponseProvider.Create(tournament, round));
			}
		}
	}
}