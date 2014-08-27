using System;
using System.Collections.Generic;
using System.Linq;
using Peregrine.Engine;
using Peregrine.Engine.Swiss;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Peregrine.Test
{
	class StatisticsAutoDataAttribute : AutoDataAttribute
	{
		public StatisticsAutoDataAttribute(int wins = 0, int losses = 0, int draws = 0)
			: base(new Fixture()
				.Customize(new ParameterNamedPlayerCustomization())
				.Customize(new ConfiguredRoundsCustomization(wins, losses, draws))
				.Customize(new AutoMoqCustomization())
			)
		{ }
	}

	class ParameterNamedPlayerCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Customizations.Add(new ParameterNamedPlayerSpecimenBuilder());
		}
	}

	class ConfiguredRoundsCustomization : ICustomization
	{
		readonly int Wins;
		readonly int Losses;
		readonly int Draws;

		public ConfiguredRoundsCustomization(int wins, int losses, int draws)
		{
			Wins = wins;
			Losses = losses;
			Draws = draws;
		}

		public void Customize(IFixture fixture)
		{
			var winner = new Player("A", null);
			var loser = new Player("B", null);

			fixture.Inject(MockDataBuilder.CreateRounds(
					new[]
					{
						new[]
						{
							Tuple.Create(winner, loser, Wins, Losses, Draws),
						}
					}
				));
		}
	}

	// Whenever you request a Player object via parameter, its name will match the parameter name
	public class ParameterNamedPlayerSpecimenBuilder : ISpecimenBuilder
	{
		public object Create(object request, ISpecimenContext context)
		{
			if(!(request is SeededRequest))
				return new NoSpecimen(request);

			var seededRequest = (SeededRequest)request;
			if(!(seededRequest.Request is Type))
				return new NoSpecimen(request);

			var requestedType = (Type)seededRequest.Request;
			if(!typeof(Player).IsAssignableFrom(requestedType))
				return new NoSpecimen(request);

			if(!(seededRequest.Seed is string))
				return new NoSpecimen(request);

			return new Player((string)seededRequest.Seed, null);
		}
	}

	class MockDataBuilder
	{
		public static IEnumerable<Round> CreateRounds(IEnumerable<IEnumerable<Tuple<Player, Player, int, int, int>>> roundResults)
		{
			return roundResults
				.Select((result, roundNumber) => CreateRound(roundNumber + 1, result));
		}

		public static Round CreateRound(int roundNumber, IEnumerable<Tuple<Player, Player, int, int, int>> roundResults)
		{
			return new Round(
					roundNumber,
					roundResults
						.Select((result, matchNumber) => CreateMatch(
								matchNumber + 1, 
								result.Item1, 
								result.Item2, 
								result.Item3, 
								result.Item4, 
								result.Item5
							))
				);
		}

		public static Match CreateMatch(int matchNumber, Player A, Player B, int winsForA, int lossesForA, int draws)
		{
			return new Match(
					matchNumber,
					new[] { A, B },
					CreateGameResults(A, B, winsForA, lossesForA, draws)
				);
		}

		public static IEnumerable<GameResult> CreateGameResults(Player A, Player B, int winsForA, int lossesForA, int draws)
		{
			return Enumerable
				.Range(0, winsForA)
				.Select(i => A)
				.Concat(Enumerable
					.Range(0, lossesForA)
					.Select(i => B))
				.Concat(Enumerable
					.Range(0, draws)
					.Select(i => (Player)null))
				.Select(player => new GameResult(player));
		}
	}

	public class SwissStatisticsProviderTests
	{
		public class When_A_Player_Wins_Two_Games
		{
			public class And_Loses_None
			{
				[Theory, StatisticsAutoData(wins: 2)]
				public void Should_Be_Awarded_Match_Win_Points(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetMatchPoints(rounds, A);

					Assert.Equal(SwissStatisticsProvider.MatchWinPoints, actual);
				}

				[Theory, StatisticsAutoData(wins: 2)]
				public void Game_Win_Percentage_Should_Be_One(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetGameWinPercentage(rounds, A);

					Assert.Equal(1.0m, actual);
				}

				[Theory, StatisticsAutoData(wins: 2)]
				public void Opponent_Match_Win_Percentage_Should_Be_Minimum(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsMatchWinPercentage(rounds, A);

					Assert.Equal(SwissStatisticsProvider.MinimumMatchWinPercentage, actual);
				}

				[Theory, StatisticsAutoData(wins: 2)]
				public void Opponent_Game_Win_Percentage_Should_Be_Minimum(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsGameWinPercentage(rounds, A);

					Assert.Equal(SwissStatisticsProvider.MinimumGameWinPercentage, actual);
				}
			}

			public class And_Loses_One
			{
				[Theory, StatisticsAutoData(wins: 2, losses: 1)]
				public void Should_Be_Awarded_Match_Win_Points(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetMatchPoints(rounds, A);

					Assert.Equal(SwissStatisticsProvider.MatchWinPoints, actual);
				}

				[Theory, StatisticsAutoData(wins: 2, losses: 1)]
				public void Game_Win_Percentage_Should_Be_Two_Thirds(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetGameWinPercentage(rounds, A);

					Assert.Equal(2.0m / 3.0m, actual);
				}

				[Theory, StatisticsAutoData(wins: 2, losses: 1)]
				public void Opponent_Match_Win_Percentage_Should_Be_Minimum(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsMatchWinPercentage(rounds, A);

					Assert.Equal(SwissStatisticsProvider.MinimumMatchWinPercentage, actual);
				}

				[Theory, StatisticsAutoData(wins: 2, losses: 1)]
				public void Opponent_Game_Win_Percentage_Should_Be_One_Third(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsGameWinPercentage(rounds, A);

					Assert.Equal(1.0m / 3.0m, actual);
				}
			}
		}

		public class When_A_Player_Loses_Two_Games
		{
			public class And_Wins_None
			{
				[Theory, StatisticsAutoData(losses: 2)]
				public void Should_Be_Awarded_No_Match_Points(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetMatchPoints(rounds, A);

					Assert.Equal(0.0m, actual);
				}

				[Theory, StatisticsAutoData(losses: 2)]
				public void Game_Win_Percentage_Should_Be_Minimum(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetGameWinPercentage(rounds, A);

					Assert.Equal(SwissStatisticsProvider.MinimumMatchWinPercentage, actual);
				}

				[Theory, StatisticsAutoData(losses: 2)]
				public void Opponent_Match_Win_Percentage_Should_Be_One(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsMatchWinPercentage(rounds, A);

					Assert.Equal(1.0m, actual);
				}

				[Theory, StatisticsAutoData(losses: 2)]
				public void Opponent_Game_Win_Percentage_Should_Be_One(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsGameWinPercentage(rounds, A);

					Assert.Equal(1.0m, actual);
				}
			}

			public class And_Wins_One
			{
				[Theory, StatisticsAutoData(wins: 1, losses: 2)]
				public void Should_Be_Awarded_No_Match_Points(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetMatchPoints(rounds, A);

					Assert.Equal(0.0m, actual);
				}

				[Theory, StatisticsAutoData(wins: 1, losses: 2)]
				public void Game_Win_Percentage_Should_Be_One_Third(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetGameWinPercentage(rounds, A);

					Assert.Equal(1.0m / 3.0m, actual);
				}

				[Theory, StatisticsAutoData(wins: 1, losses: 2)]
				public void Opponent_Match_Win_Percentage_Should_Be_One(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsMatchWinPercentage(rounds, A);

					Assert.Equal(1.0m, actual);
				}

				[Theory, StatisticsAutoData(wins: 1, losses: 2)]
				public void Opponent_Game_Win_Percentage_Should_Be_Two_Thirds(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsGameWinPercentage(rounds, A);

					Assert.Equal(2.0m / 3.0m, actual);
				}
			}
		}

		public class When_There_Is_A_Draw
		{
			public class And_Neither_Player_Has_Won_A_Game
			{
				[Theory, StatisticsAutoData(draws: 1)]
				public void Should_Be_Awarded_Draw_Match_Point(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetMatchPoints(rounds, A);

					Assert.Equal(SwissStatisticsProvider.MatchDrawPoints, actual);
				}

				[Theory, StatisticsAutoData(draws: 1)]
				public void Game_Win_Percentage_Should_Be_One_Third(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetGameWinPercentage(rounds, A);

					Assert.Equal(1.0m / 3.0m, actual);
				}

				[Theory, StatisticsAutoData(draws: 1)]
				public void Opponent_Match_Win_Percentage_Should_Be_One_Third(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsMatchWinPercentage(rounds, A);

					Assert.Equal(1.0m / 3.0m, actual);
				}

				[Theory, StatisticsAutoData(draws: 1)]
				public void Opponent_Game_Win_Percentage_Should_Be_One_Third(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsGameWinPercentage(rounds, A);

					Assert.Equal(1.0m / 3.0m, actual);
				}
			}

			public class And_Both_Players_Have_Won_A_Game
			{
				[Theory, StatisticsAutoData(wins: 1, losses: 1, draws: 1)]
				public void Should_Be_Awarded_Draw_Match_Points(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetMatchPoints(rounds, A);

					Assert.Equal(SwissStatisticsProvider.MatchDrawPoints, actual);
				}

				[Theory, StatisticsAutoData(wins: 1, losses: 1, draws: 1)]
				public void Game_Win_Percentage_Should_Be_Four_Ninths(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetGameWinPercentage(rounds, A);

					// One win + one draw = 4GP out of 9 possible
					Assert.Equal(4.0m / 9.0m, actual);
				}

				[Theory, StatisticsAutoData(wins: 1, losses: 1, draws: 1)]
				public void Opponent_Match_Win_Percentage_Should_Be_Minimum(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsMatchWinPercentage(rounds, A);

					Assert.Equal(SwissStatisticsProvider.MinimumMatchWinPercentage, actual);
				}

				[Theory, StatisticsAutoData(wins: 1, losses: 1, draws: 1)]
				public void Opponent_Game_Win_Percentage_Should_Be_Four_Ninths(SwissStatisticsProvider sut, IEnumerable<Round> rounds, Player A, Player B)
				{
					var actual = sut
						.GetOpponentsGameWinPercentage(rounds, A);

					// One win + one draw = 4GP out of 9 possible
					Assert.Equal(4.0m / 9.0m, actual);
				}
			}
		}

		// Test pairings
		public class When_Pairing_Players
		{
			public class Before_The_First_Round
			{
				public class When_There_Is_An_Even_Number_Of_Players
				{
					[Theory, StatisticsAutoData]
					public void All_Players_Should_Be_Paired(SwissRankingEngine sut, Player A, Player B, Player C, Player D)
					{
						// Seed 7357 = Round 1 pairing order { B, C, A, D }
						var context = new SwissTournamentContext(
							7357,
							TournamentState.TournamentCreated,
							new[] { A, B, C, D },
							null,
							null);

						var actual = sut.GetPairings(context);

						Assert.Equal(actual.Count(), 2);

						var firstPairing = actual.ElementAt(0);
						Assert.Equal(2, firstPairing.Players.Count());
						Assert.Contains(B, firstPairing.Players);
						Assert.Contains(C, firstPairing.Players);

						var secondPairing = actual.ElementAt(1);
						Assert.Equal(2, secondPairing.Players.Count());
						Assert.Contains(A, secondPairing.Players);
						Assert.Contains(D, secondPairing.Players);
					}
				}

				public class When_There_Is_An_Uneven_Number_Of_Players
				{
					[Theory, StatisticsAutoData]
					public void One_Player_Should_Get_A_Bye(SwissRankingEngine sut, Player A, Player B, Player C)
					{
						// Seed 7357 = Round 1 pairing order { B, C, A }
						var context = new SwissTournamentContext(
							7357,
							TournamentState.TournamentCreated,
							new[] { A, B, C },
							null,
							null);

						var actual = sut.GetPairings(context);

						Assert.Equal(actual.Count(), 2);

						var firstPairing = actual.ElementAt(0);
						Assert.Equal(2, firstPairing.Players.Count());
						Assert.Contains(B, firstPairing.Players);
						Assert.Contains(C, firstPairing.Players);

						var byePairing = actual.ElementAt(1);
						Assert.Equal(1, byePairing.Players.Count());
						Assert.Contains(A, byePairing.Players);
					}
				}
			}
		}
	}
}
