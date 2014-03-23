using System;
using System.Linq;
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

				var roundState = RoundManager.DetermineRoundState(tournament, roundNumber);
				if(roundState == RoundState.Invalid)
					return NotFound();

				// Can't add results to finalized rounds
				if(roundState == RoundState.Finalized)
					return StatusCode(System.Net.HttpStatusCode.MethodNotAllowed);

				var round = tournament
					.Rounds
					.Where(r => r.Number == roundNumber)
					.FirstOrDefault()
					?? new Round
					{
						Number= roundNumber,
						Matches = RoundManager.CreateMatches(tournament, roundNumber)
					};

				if(roundState == RoundState.Projected)
				{
					// Add the new round to the tournament.
					tournament.Rounds.Add(round);
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
		//	using(var dataContext = new DataContext())
		//	{
		//		var tournament = dataContext
		//			.GetTournament(tournamentKey);

		//		if(tournament == null)
		//			return NotFound();

		//		var player = tournament
		//			.GetPlayer(playerName);

		//		if(player == null)
		//			return NotFound();

		//		var roundState = RoundManager.DetermineRoundState(tournament, roundNumber);
		//		if(roundState == RoundState.Invalid)
		//			return NotFound();

		//		// Can't add results to finalized rounds
		//		if(roundState == RoundState.Finalized)
		//			return StatusCode(System.Net.HttpStatusCode.MethodNotAllowed);

		//		var round = tournament
		//			.Rounds
		//			.Where(r => r.Number == roundNumber)
		//			.FirstOrDefault()
		//			?? new Round
		//			{
		//				Number = roundNumber,
		//				Matches = RoundManager.CreateMatches(tournament, roundNumber)
		//			};

		//		if(roundState == RoundState.Projected)
		//		{
		//			// Add the new round to the tournament.
		//			tournament.Rounds.Add(round);
		//		}

		//		// Committed and Completed rounds accept new results.
		//		// Find the match for this player
		//		var match = tournament
		//			.GetRound(roundNumber)
		//			.GetMatch(player);

		//		if(match == null)
		//			return NotFound();

		//		// Clear all the winning games for this player
		//		var existingGames = match
		//			.Games
		//			.Where(game => game. == player)
		//			.ToArray();

		//		foreach(var game in existingGames)
		//			match.Games.Remove(game);

		//		// Add new winning games
		//		for(int draws = 0; draws < count; draws++)
		//			match
		//				.Games
		//				.Add(new Game
		//				{
		//					Winner = player,
		//				});

		//		dataContext.SaveChanges();

		//		return Ok(new
		//		{
		//			players = match
		//				.Players
		//				.Select(p => p.Name)
		//				.ToArray()
		//		});
		//	}
		//}
	}
}