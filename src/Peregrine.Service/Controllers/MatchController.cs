using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/round/{roundNumber}")]
	public class MatchController : ApiController
    {
		public enum MatchResult
		{
			Win,
			Draw,
		};

		[Route("matches", Name = "Match.List")]
		public IHttpActionResult GetList(Guid key, int roundNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.FirstOrDefault(t => t.Key == key);

				if(tournament == null)
					return NotFound();

				var round = tournament
					.Rounds
					.Where(r => r.Number == roundNumber)
					.FirstOrDefault();

				if(round == null)
					return NotFound();

				return Ok(new
				{
					matches = round
						.Matches
						.OrderBy(m => m.Number)
						.Select(m => new
						{
							_link = Url.Link("Match.Get", new { key = tournament.Key, roundNumber = round.Number, matchNumber = m.Number }),
							number = m.Number,
							players = m
								.Players
								.OrderBy(p => p.Name)
								.Select(p => new
								{
									_link = Url.Link("Player.Get", new { key = tournament.Key, name = p.Name }),
									name = p.Name,
								})
								.ToArray(),
							results = m
								.Games
								.OrderBy(g => g.Number)
								.Select(g => new
								{
									number = g.Number,
									winner = g.Winner == null ? null : new
									{
										_link = Url.Link("Player.Get", new { key = tournament.Key, name = g.Winner.Name }),
										name = g.Winner.Name,
									}
								})
								.ToArray(),
						})	
				});
			}
		}

		[Route("match/{matchNumber}", Name = "Match.Get")]
		public IHttpActionResult Get(Guid key, int roundNumber, int matchNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.FirstOrDefault(t => t.Key == key);

				if(tournament == null)
					return NotFound();

				var round = tournament
					.Rounds
					.Where(r => r.Number == roundNumber)
					.FirstOrDefault();

				if(round == null)
					return NotFound();

				var match = round
					.Matches
					.Where(m => m.Number == matchNumber)
					.FirstOrDefault();

				if(match == null)
					return NotFound();

				return Ok(new
				{
					_link = Url.Link("Match.Get", new { key = tournament.Key, roundNumber = round.Number, matchNumber = match.Number }),
					number = match.Number,
					players = match
						.Players
						.OrderBy(p => p.Name)
						.Select(p => new
						{
							_link = Url.Link("Player.Get", new { key = tournament.Key, name = p.Name }),
							name = p.Name,
						})
						.ToArray(),
					results = match
						.Games
						.OrderBy(g => g.Number)
						.Select(g => new
						{
							number = g.Number,
							winner = g.Winner == null ? null : new
							{
								_link = Url.Link("Player.Get", new { key = tournament.Key, name = g.Winner.Name }),
								name = g.Winner.Name,
							}
						})
						.ToArray(),

				});
			}
		}

		[Route("{name}/{result}", Name = "MatchResult.Put")]
		public IHttpActionResult Put(Guid key, int roundNumber, string name, MatchResult result)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.FirstOrDefault(t => t.Key == key);

				if(tournament == null)
					return NotFound();

				var round = tournament
					.Rounds
					.Where(r => r.Number == roundNumber)
					.FirstOrDefault();

				if(round == null)
					return NotFound();

				var player = tournament
					.Players
					.Where(p => p.Name == name)
					.FirstOrDefault();

				var match = round
					.Matches
					.Where(m => m.Players.Contains(player))
					.FirstOrDefault();

				if(match == null)
					return NotFound();

				// Null winner indicates draw
				match.Games.Add(new Game
				{
					Number = match.Games.Count + 1,
					Winner = result == MatchResult.Win ? player : null,
				});

				dataContext.SaveChanges();

				return Ok(new
				{
					_link = Url.Link("Match.Get", new { key = tournament.Key, roundNumber = round.Number, matchNumber = match.Number }),
					number = match.Number,
					players = match
						.Players
						.OrderBy(p => p.Name)
						.Select(p => new
						{
							_link = Url.Link("Player.Get", new { key = tournament.Key, name = p.Name }),
							name = p.Name,
						})
						.ToArray(),
					results = match
						.Games
						.OrderBy(g => g.Number)
						.Select(g => new
						{
							number = g.Number,
							winner = g.Winner == null ? null : new
							{
								_link = Url.Link("Player.Get", new { key = tournament.Key, name = g.Winner.Name }),
								name = g.Winner.Name,
							}
						})
						.ToArray(),
				});
			}
		}
    }
}
