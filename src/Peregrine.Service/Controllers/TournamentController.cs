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

				return CreatedAtRoute("Tournament.Get", new { key = tournament.Key }, tournament);
			}
		}

		[Route("tournament/{key}", Name = "Tournament.Get")]
		public IHttpActionResult Get(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				dataContext.Configuration.LazyLoadingEnabled = false;

				var tournament = dataContext.Tournaments.FirstOrDefault(t => t.Key == key);
				if(tournament == null)
					return NotFound();

				return Ok(tournament);
			}
		}
    }
}
