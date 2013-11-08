using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Peregrine.Data;

namespace Peregrine.Service
{
	public static class ApiControllerModelRenderExtensions
	{
		public static JObject RenderDetail(this ApiController controller, Tournament tournament)
		{
			return JObject.FromObject(new
			{
				_link = controller.Url.Link("Tournament.Get", new { key = tournament.Key }),
				key = tournament.Key,
				players = controller.RenderDetail(tournament.Players, tournament.Key),
				rounds = controller.RenderDetail(tournament.Rounds, tournament.Key),
			});
		}

		public static JObject RenderSummary(this ApiController controller, Tournament tournament)
		{
			return JObject.FromObject(new
			{
				key = tournament.Key,
			});
		}

		public static JObject RenderDetail(this ApiController controller, IEnumerable<Player> players, Guid tournamentKey)
		{
			return JObject.FromObject(new
			{
				_link = controller.Url.Link("Players.Get", new { key = tournamentKey }),
				players = (players ?? Enumerable.Empty<Player>())
					.OrderBy(p => p.Name)
					.Select(p => controller.RenderDetail(p, tournamentKey)),
			});
		}

		public static JObject RenderDetail(this ApiController controller, Player player, Guid tournamentKey)
		{
			return JObject.FromObject(new
			{
				_link = controller.Url.Link("Player.Get", new { key = tournamentKey, name = player.Name }),
				name = player.Name,
			});
		}

		public static JObject RenderDetail(this ApiController controller, IEnumerable<Round> rounds, Guid tournamentKey)
		{
			return JObject.FromObject(new
			{
				_link = controller.Url.Link("Rounds.Get", new { key = tournamentKey }),
				rounds = (rounds ?? Enumerable.Empty<Round>())
					.OrderBy(r => r.Number)
					.Select(r => controller.RenderDetail(r, tournamentKey)),
			});
		}

		public static JObject RenderDetail(this ApiController controller, Round round, Guid tournamentKey)
		{
			return JObject.FromObject(new
			{
				_link = controller.Url.Link("Round.Get", new { key = tournamentKey, roundNumber = round.Number }),
				matches = controller.RenderDetail(round.Matches, tournamentKey, round.Number),
			});
		}

		public static JObject RenderDetail(this ApiController controller, IEnumerable<Match> matches, Guid tournamentKey, int roundNumber)
		{
			return JObject.FromObject(new
			{
				_link = controller.Url.Link("Matches.Get", new { key = tournamentKey, roundNumber = roundNumber }),
				matches = (matches ?? Enumerable.Empty<Match>())
					.OrderBy(m => m.Number)
					.Select(m => controller.RenderDetail(m, tournamentKey, roundNumber))
			});
		}

		public static JObject RenderDetail(this ApiController controller, Match match, Guid tournamentKey, int roundNumber)
		{
			return JObject.FromObject(new
			{
				_link = controller.Url.Link("Match.Get", new { key = tournamentKey, roundNumber = roundNumber, matchNumber = match.Number }),
				number = match.Number,
				players = controller.RenderDetail(match.Players, tournamentKey),
				results = controller.RenderDetail(match.Games, tournamentKey),
			});
		}

		public static JObject RenderDetail(this ApiController controller, IEnumerable<Game> games, Guid tournamentKey)
		{
			return JObject.FromObject(new
			{
				results = (games ?? Enumerable.Empty<Game>())
					.OrderBy(g => g.Number)
					.Select(g => controller.RenderDetail(g, tournamentKey))
			});
		}


		public static JObject RenderDetail(this ApiController controller, Game game, Guid tournamentKey)
		{
			return JObject.FromObject(new
			{
				number = game.Number,
				winner = game.Winner == null ? null : new
				{
					_link = controller.Url.Link("Player.Get", new { key = tournamentKey, name = game.Winner.Name }),
					name = game.Winner.Name,
				}
			});
		}
	}
}