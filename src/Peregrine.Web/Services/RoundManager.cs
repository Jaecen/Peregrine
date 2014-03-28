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
		readonly StatsProvider StatsProvider;

		public RoundManager()
		{
			StatsProvider = new StatsProvider();
		}

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

			if(tournament.Players.Count < 2)
				return RoundState.Invalid;

			var previousRoundState = DetermineRoundState(tournament, roundNumber - 1);

			if(previousRoundState != RoundState.Completed && previousRoundState != RoundState.Finalized)
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
			var seed = tournament.Seed ^ roundNumber;
			var rng = new Random(seed);

			var tournamentRounds = tournament
				.Rounds
				.SelectMany(round => round.Matches)
				.ToArray();

			// Group players by match points.
			// Randomize within each group.
			var playerPool = tournament
				.Players
				.Select(player => new
					{
						Player = player,
						Randomizer = rng.Next(),
						MatchPoints = StatsProvider.GetMatchPoints(tournament, player),
						HadBye = tournamentRounds
							.Where(match => match.Players.Contains(player))
							.Where(match => match.Players.Count == 1)
							.Any(),
						Opponents = tournamentRounds
							.Where(match => match.Players.Contains(player))
							.SelectMany(match => match.Players.Except(new[] { player }))
							.ToArray()
					})
				.OrderBy(o => o.MatchPoints)
				.ThenBy(o => o.Randomizer)
				.ToArray()
				.AsEnumerable();

			var matches = Enumerable.Empty<Match>();

			// If there is an odd number of players, from the bottom up, take the first player 
			// that hasn't had a bye and remove them from the pool.
			if(playerPool.Count() % 2 == 1)
			{
				var byePlayer = playerPool
					.Reverse()
					.Where(o => !o.HadBye)
					.Take(1);

				matches = matches
					.Concat(byePlayer
						.Select(o => new Match
						{
							// A bye gives two game wins
							Games = new[]
								{
									new Game
									{
										Winner = o.Player,
									},
									new Game
									{
										Winner = o.Player,
									},
								},
							Players = new[] 
								{ 
									o.Player,
								},
						})
					);

				playerPool = playerPool.Except(byePlayer);
			}

			// Start at the top and take the next player in order. If they've never had a match, 
			// place both in a pairing and remove them from the pool. If they have had a match,
			// take the next lowest player. Repeat until an unplayed opponent is found.
			while(playerPool.Any())
			{
				var left = playerPool
					.First();

				var right = playerPool
					.Skip(1)
					.SkipWhile(o => left
						.Opponents
						.Contains(o.Player)
					)
					.FirstOrDefault()
					?? playerPool
						.Skip(1)
						.First();

				matches = matches
					.Concat(new[]
						{
							new Match
							{
								Games = new List<Game>(),
								Players = new[] 
									{ 
										left.Player, 
										right.Player,
									}
									.OrderBy(player => player.Name)
									.ToArray(),
							}
						}
					);

				playerPool = playerPool
					.Except(new[] { left, right });
			}

			return matches.ToArray();
		}

		public object RenderRound(Round round, RoundState roundState)
		{
			return new
			{
				number = round.Number,
				started = roundState == RoundState.Committed,
				completed = roundState == RoundState.Completed || roundState == RoundState.Finalized,
				final = roundState == RoundState.Finalized,
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
								dropped = p.Dropped,
							})
					})
					.ToArray()
			};
		}

		public int GetMaxRoundsForTournament(Tournament tournament)
		{
			var playerCount = tournament.Players.Count();
			var roundCount = Math.Ceiling(Math.Log(playerCount, 2));

			// No less than one round
			return (int)Math.Max(1, roundCount);
		}
	}
}