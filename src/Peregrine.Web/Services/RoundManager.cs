using System;
using System.Collections.Generic;
using System.Linq;
using Peregrine.Data;

namespace Peregrine.Web.Services
{
	enum RoundState
	{
		Projected,
		Committed,
		Completed,
		Finalized,
		Invalid,
	}

	class RoundManager
	{
		// Rounds have five states: projected, committed, and completed, finalized, and invalid
		// - A round is finalized when it's been completed and a result entered for the next round
		// - A round is completed when all matches have the minimum number of results
		// - A round is committed when the first result is submitted. A result can't be submitted until the previous round is completed.
		// - A round is projected when the previous round is completed by no results have been submitted.
		// - A round is invalid if it's more than one greater than the last completed round number
		// When a round becomes committed, it's written to the database and can't be changed. All matches must have a result submitted to move forward.
		public RoundState DetermineRoundState(Tournament tournament, int roundNumber)
		{
			if(roundNumber < 0)
				return RoundState.Invalid;

			if(roundNumber == 0)
				return RoundState.Completed;

			var previousRoundState = DetermineRoundState(tournament, roundNumber - 1);

			if(previousRoundState != RoundState.Completed)
				return RoundState.Invalid;

			var requestedRound = tournament
				.Rounds
				.Where(round => round.Number == roundNumber)
				.FirstOrDefault();

			if(requestedRound == null)
				return RoundState.Projected;

			// A round is completed when all matches have at least two wins or are called because of time.
			// Since we don't track called games, we just check for at least one completed game in each match.
			var requestedRoundIsCompleted = requestedRound
				.Matches
				.All(match => match
					.Games
					.Count() >= 1
				);

			var nextRoundHasResults = tournament
				.Rounds
				.Where(round => round.Number == roundNumber + 1)
				.Where(round => round.Matches.Any())
				.Any();

			if(requestedRoundIsCompleted)
				if(nextRoundHasResults)
					return RoundState.Finalized;
				else
					return RoundState.Completed;

			return RoundState.Committed;
		}

		public ICollection<Match> CreateMatches(Tournament tournament, int roundNumber)
		{
			var rng = new Random(tournament.Seed);

			IEnumerable<Player> players;
			if(roundNumber == 1)
			{
				// Completely random
				players = tournament
					.Players
					.Where(player => !player.Dropped)
					.Select(player => new
					{
						Player = player,
						Order = rng.Next(),
					})
					.OrderBy(o => o.Order)
					.Select(o => o.Player)
					.ToArray();
			}
			else
			{
				// Take previous round, sort by match points, random within same number of points.
				// Odd numbers pair up to the next highest match points
				throw new NotImplementedException();
			}
				
			var evens = players
				.Where((player, index) => index % 2 == 0)
				.ToArray();

			var odds = players
				.Where((player, index) => index % 2 == 1)
				.ToArray();

			var byes = evens
				.Skip(odds.Count())
				.ToArray();
			
			var pairings = evens
				.Zip(odds,
					(left, right) => new Match
					{
						Games = new List<Game>(),
						Players = new[] 
							{ 
								left, 
								right,
							},
					}
				)
				.Concat(byes
					.Select(player => new Match
					{
						// A bye gives two game wins
						Games = new[]
							{
								new Game
								{
									Winner = player,
								},
								new Game
								{
									Winner = player,
								},
							},
						Players = new[] 
							{ 
								player, 
							},
					}))
				.ToArray();

			return pairings;
		}

		public object RenderRound(Round round, RoundState roundState)
		{
			return new
			{
				number = round.Number,
				completed = roundState == RoundState.Completed || roundState == RoundState.Finalized,
				matches = round
					.Matches
					.Select(m => new
					{
						games = m.Games.Count(),
						players = m.Players
							.Select(p => new
							{
								name = p.Name,
								wins = m.Games.Where(g => g.Winner == p).Count(),
								losses = m.Games.Where(g => g.Winner != p && g.Winner != null).Count(),
								draws = m.Games.Where(g => g.Winner == null).Count(),
							})
					})
					.ToArray()
			};
		}
	}
}