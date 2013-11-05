using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
    public class TournamentController : ApiController
    {
		[Route("tournament", Name="Tournament.Post")]
		public IHttpActionResult Post()
		{
			using(var dataContext = new DataContext())
			{
				var tournament = new Tournament
				{
					Key = Guid.NewGuid(),
				};

				dataContext.Tournaments.Add(tournament);
				dataContext.SaveChanges();

				return CreatedAtRoute("Tournament.Get", new { key = tournament.Key }, new
				{
					_link = Url.Link("Tournament.Get", new { key = tournament.Key }),
					key = tournament.Key,
					players = new Player[0],
					rounds = new Round[0],
				});
			}
		}

		[Route("tournament/{key}", Name = "Tournament.Get")]
		public IHttpActionResult Get(Guid key)
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
					_link = Url.Link("Tournament.Get", new { key = tournament.Key }),
					key = tournament.Key,
					players = tournament
						.Players
						.OrderBy(p => p.Name)
						.Select(p => new
						{
							_link = Url.Link("Player.Get", new { key = tournament.Key, name = p.Name }),
							name = p.Name,
						})
						.ToArray(),
					rounds = tournament
						.Rounds
						.OrderBy(r => r.Number)
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
    }
}
