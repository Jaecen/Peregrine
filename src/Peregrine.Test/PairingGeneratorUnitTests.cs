using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Peregrine.Engine;
using Peregrine.Engine.Swiss;
using Xunit;

namespace Peregrine.Test
{
	public class PairingGeneratorUnitTests
	{
		[Fact]
		public void Should_Generate_All_Possible_Pairings()
		{
			var generator = new SwissRankingEngine(new SwissStatisticsProvider());

			var players = Enumerable
				.Range(0, 26)
				.Select(number  => new String(new[] { (char)('a' + number) }))
				.Select(name => new Player(name, null));

			// Careful with the playerCount. 12 will take almost a minute to execute.
			for(int playerCount = 2; playerCount <= 10; playerCount += 2)
			{
				var matchSets = generator.GeneratePairedMatches(players.Take(playerCount), Enumerable.Empty<MatchResult>(), 1);

				var uniquePairings = matchSets
					.Select(matchSet => matchSet
						.Select(match => match
							.Players
							.OrderBy(player => player.Identifier)
							.Select(player => player.Identifier)
						)
						.Select(playerIdentifiers => String.Join("-", playerIdentifiers))
						.OrderBy(pairing => pairing)
					)
					.Select(playerIdentifiers => String.Join("|", playerIdentifiers))
					.OrderBy(matchSetPairings => matchSetPairings)
					.Distinct()
					.ToArray();

				// Expected number of pairings is (2n - 1)!! where n = playerCount / 2
				// See http://en.wikipedia.org/wiki/Perfect_matching and http://en.wikipedia.org/wiki/Double_factorial for reasoning 
				// See http://mathworld.wolfram.com/DoubleFactorial.html for the definition of (2n - 1)!! used here
				var expectedUniquePairings = (int)(Factorial(playerCount) / (Math.Pow(2, playerCount / 2) * Factorial(playerCount / 2)));
				var actualUniquePairings = uniquePairings.Count();
				Assert.Equal(expectedUniquePairings, actualUniquePairings);
			}
		}

		int Factorial(int n)
		{
			if(n == 0)
				return 1;
			else
				return n * Factorial(n - 1);
		}
	}
}
