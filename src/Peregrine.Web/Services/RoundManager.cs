using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

			var requestedRoundIsCompleted = requestedRound
				.Matches
				.All(match => match
					.Games
					.Count() >= Tournament.MinGamesPerRound
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

			if(roundNumber == 1)
			{
				var players = tournament
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

				var pairs = players
					.Where((player, index) => index % 2 == 0)
					.Zip(
						players
							.Where((player, index) => index % 2 == 1),
						(left, right) => new Match
						{
							Players = new[] 
								{ 
									left, 
									right
								},
						}
					)
					.ToArray();

				return pairs;
			}

			// Take previous round, sort by match points, random within same number of points.
			// Odd numbers pair up to the next highest match points
			throw new NotImplementedException();
		}
	}
}