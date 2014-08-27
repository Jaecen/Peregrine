using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peregrine.Engine;
using Peregrine.Engine.Swiss;
using Xunit;

namespace Peregrine.Test
{
	public class EventHandlingIntegrationTests
	{
		[Fact]
		public void It_Just_Freaking_Works()
		{
			// Yay happy path!

			var statisticsProvider = new SwissStatisticsProvider();
			var rankingEngine = new SwissRankingEngine(statisticsProvider);
			var contextBuilder = new SwissTournamanetContextBuilder(statisticsProvider, rankingEngine);
			var commandEventHandler = new CommandEventHandler<SwissTournamentContext>(contextBuilder);

			var context = new SwissTournamentContext(0, TournamentState.None, null, null, null);

			// Create the tournament
			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0, 
					aggregateId: "T1",
					name: TournamentCommand.CreateTournament.ToString(), 
					properties: new Dictionary<string, string>
					{
						{ "Timestamp", new DateTimeOffset(1982, 7, 22, 01, 14, 22, TimeSpan.FromHours(-7)).ToString() }
					})
			);

			Assert.Equal(TournamentState.TournamentCreated, context.State);
			Assert.NotEqual(0, context.TournamentSeed);

			// Add players
			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.AddPlayer.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "A" }
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.AddPlayer.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "B" }
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.AddPlayer.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "C" }
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.AddPlayer.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "D" }
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.AddPlayer.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "E" }
					})
			);

			Assert.Equal(TournamentState.TournamentStarted, context.State);
			Assert.NotEmpty(context.Players);

			var roundOnePairings = rankingEngine.GetPairings(context);

			// Record match results for round 1
			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "A" },
						{ "Wins", "2" },
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "B" },
						{ "Wins", "1" },
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "D" },
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "E" },
						{ "Draws", "1" },
					})
			);

			// End round 1
			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.EndRound.ToString(),
					properties: new Dictionary<string, string>
					{
					})
			);

			// Get standings as of round 1. If we include round 2, we get the bye points immediately.
			var roundOneStandings = rankingEngine.GetStandings(context, 1);
			var roundTwoPairings = rankingEngine.GetPairings(context);

			// Record match results for round 2
			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "A" },
						{ "Draws", "1" },
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "C" },
						{ "Wins", "2" },
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "D" },
						{ "Wins", "1" },
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "E" },
					})
			);

			// End round 2
			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.EndRound.ToString(),
					properties: new Dictionary<string, string>
					{
					})
			);

			var roundTwoStandings = rankingEngine.GetStandings(context, 2);
			
			// Drop a player
			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RemovePlayer.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "E" },
						{ "Wins", "1" },
					})
			);

			// Record match results for round 3
			var roundThreePairings = rankingEngine.GetPairings(context);

			// Record match results for round 3
			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "A" },
						{ "Wins", "1" },
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "B" },
						{ "Draws", "1" },
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "C" },
					})
			);

			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.RecordMatchResults.ToString(),
					properties: new Dictionary<string, string>
					{
						{ "Player", "D" },
						{ "Wins", "2" },
					})
			);

			// End round 3 and the tournament
			context = commandEventHandler.ProcessCommand(
				context: context,
				commandEvent: new CommandEvent(
					sequence: 0,
					aggregateId: "T1",
					name: TournamentCommand.EndRound.ToString(),
					properties: new Dictionary<string, string>
					{
					})
			);

			var finalStandings = rankingEngine.GetStandings(context);
		}
	}
}
