using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}")]
	public class RoundController : ApiController
    {
		[Route("rounds", Name = "Round.List")]
		public IHttpActionResult GetList(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.FirstOrDefault(t => t.Key == key);

				if(tournament == null)
					return NotFound();

				return Ok(new
				{
					rounds = tournament
						.Rounds
						.OrderBy(r => r.Number)
						.DefaultIfEmpty(new Round
						{
							Number = 1,
							Matches = new List<Match>(),
						})
						.Select(r => new
						{
							_link = Url.Link("Round.Get", new { key = tournament.Key, roundNumber = r.Number }),
							matches = r
								.Matches
								.OrderBy(m => m.Number)
								.Select(m => new
								{
									_link = Url.Link("Match.Get", new { key = tournament.Key, roundNumber = r.Number, matchNumber = m.Number }),
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
								})
								.ToArray(),
						})
						.ToArray(),
				});
			}
		}

		[Route("round/{roundNumber}", Name = "Round.Get")]
		public IHttpActionResult Get(Guid key, int roundNumber)
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
					if(roundNumber == 1)
						round = GenerateFirstRound(tournament);
					else
						return NotFound();

				return Ok(new
				{
					_link = Url.Link("Round.Get", new { key = tournament.Key, roundNumber = round.Number }),
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
						})
						.ToArray(),
				});
			}
		}

		private Round GenerateFirstRound(Tournament tournament)
		{
			// Generate initial order by hashing tournament key + name
			return new Round
			{
				Number = 1,
				Matches = tournament
					.Players
					.OrderBy(p => String.Format("{0}{1}{2}", tournament.Key, p.Name, tournament.Players.Count).ComputeMd5())
					.PartitionBy(2)
					.Select((partition, index) => new Match
					{
						Number = index + 1,
						Players = partition.ToArray(),
					})
					.ToArray()
			};
		}
    }
}
