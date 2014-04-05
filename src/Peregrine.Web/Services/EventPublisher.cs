using System;
using Peregrine.Data;
using Peregrine.Web.Models;

namespace Peregrine.Web.Services
{
	public class EventPublisher
	{
		readonly TournamentResponseProvider TournamentResponseProvider;
		readonly PlayerResponseProvider PlayerResponseProvider;
		readonly RoundResponseProvider RoundResponseProvider;
		readonly TournamentManager TournamentManager;
		readonly RoundManager RoundManager;
		readonly StandingsResponseProvider StandingsResponseProvider;

		public EventPublisher(TournamentResponseProvider tournamentResponseProvider, PlayerResponseProvider playerResponseProvider, RoundResponseProvider roundResponseProvider, TournamentManager tournamentManager, RoundManager roundManager, StandingsResponseProvider standingsResponseProvider)
		{
			if(tournamentResponseProvider == null)
				throw new ArgumentNullException("tournamentResponseProvider");

			if(playerResponseProvider == null)
				throw new ArgumentNullException("playerResponseProvider");

			if(roundResponseProvider == null)
				throw new ArgumentNullException("roundResponseProvider");

			if(roundManager == null)
				throw new ArgumentNullException("roundManager");

			if(tournamentManager == null)
				throw new ArgumentNullException("tournamentManager");

			if(standingsResponseProvider == null)
				throw new ArgumentNullException("standingsResponseProvider");

			TournamentResponseProvider = tournamentResponseProvider;
			PlayerResponseProvider = playerResponseProvider;
			RoundResponseProvider = roundResponseProvider;
			TournamentManager = tournamentManager;
			RoundManager = roundManager;
			StandingsResponseProvider = standingsResponseProvider;
		}

		public void Created(Tournament tournament)
		{
			Publish("created", TournamentResponseProvider.Create(tournament));
		}

		public void Created(Tournament tournament, Player player)
		{
			Publish("created", PlayerResponseProvider.Create(player), tournament.Key, player.Name);

			Updated(tournament);
			Updated(tournament, RoundManager.GetRound(tournament, 1));
		}

		public void Created(Tournament tournament, Round round)
		{
			Publish("created", RoundResponseProvider.Create(tournament, round), tournament.Key, round.Number);
			Updated(tournament);
		}

		public void Updated(Tournament tournament)
		{
			Publish("updated", TournamentResponseProvider.Create(tournament));
		}

		public void Updated(Tournament tournament, Player player)
		{
			Publish("updated", PlayerResponseProvider.Create(player), tournament.Key, player.Name);
			Updated(tournament);

			for(var roundNumber = 1; roundNumber < (TournamentManager.GetCurrentRoundNumber(tournament) ?? 0); roundNumber++)
			{
				Updated(tournament, RoundManager.GetRound(tournament, roundNumber));
				UpdateStandings(tournament, roundNumber);
			}

			UpdateStandings(tournament);
		}

		public void Updated(Tournament tournament, Round round)
		{
			Publish("updated", RoundResponseProvider.Create(tournament, round), tournament.Key, round.Number);
			Updated(tournament);
			UpdateStandings(tournament, round.Number);
			UpdateStandings(tournament);
		}

		public void UpdateStandings(Tournament tournament, int? roundNumber = null)
		{
			Publish("updated", StandingsResponseProvider.Create(tournament, roundNumber), tournament.Key, roundNumber);
		}

		public void Deleted(Tournament tournament)
		{
			Publish("deleted", TournamentResponseProvider.Create(tournament));
		}

		public void Deleted(Tournament tournament, Player player)
		{
			Publish("deleted", PlayerResponseProvider.Create(player), tournament.Key, player.Name);
			Updated(tournament);

			var currentRoundNumber = TournamentManager.GetCurrentRoundNumber(tournament);
			if(currentRoundNumber.HasValue)
			{
				Updated(tournament, RoundManager.GetRound(tournament, currentRoundNumber.Value));
				UpdateStandings(tournament, currentRoundNumber);
			}

			UpdateStandings(tournament);
		}

		public void Deleted(Tournament tournament, Round round)
		{
			Publish("deleted", RoundResponseProvider.Create(tournament, round), tournament.Key, round.Number);
		}

		void Publish(string eventName, TournamentResponse tournamentResponse)
		{
			if(String.IsNullOrEmpty(eventName))
				throw new ArgumentException("Must not be null or empty", "eventName");

			if(tournamentResponse == null)
				throw new ArgumentNullException("tournamentResponse");

			EventStreamManager
				.GetInstance(String.Format("tournament/{0}", tournamentResponse.key))
				.Publish(eventName, tournamentResponse);
		}

		void Publish(string eventName, PlayerResponse playerResponse, Guid tournamentKey, string playerName)
		{
			if(String.IsNullOrEmpty(eventName))
				throw new ArgumentException("Must not be null or empty", "eventName");

			if(playerResponse == null)
				throw new ArgumentNullException("playerResponse");

			EventStreamManager
				.GetInstance(String.Format("player/{0}/{1}", tournamentKey, playerName))
				.Publish(eventName, playerResponse);
		}

		void Publish(string eventName, RoundResponse roundResponse, Guid tournamentKey, int roundNumber)
		{
			if(String.IsNullOrEmpty(eventName))
				throw new ArgumentException("Must not be null or empty", "eventName");

			if(roundResponse == null)
				throw new ArgumentNullException("roundResponse");

			EventStreamManager
				.GetInstance(String.Format("round/{0}/{1}", tournamentKey, roundNumber))
				.Publish(eventName, roundResponse);
		}

		void Publish(string eventName, StandingsResponse standingsResponse, Guid tournamentKey, int? roundNumber)
		{
			if(String.IsNullOrEmpty(eventName))
				throw new ArgumentException("Must not be null or empty", "eventName");

			if(standingsResponse == null)
				throw new ArgumentNullException("standingsResponse");

			EventStreamManager
				.GetInstance(String.Format("standings/{0}/{1}", tournamentKey, roundNumber))
				.Publish(eventName, standingsResponse);
		}
	}
}