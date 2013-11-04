using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
    public class TournamentController : ApiController
    {
		[Route("tournament")]
		public Tournament Post()
		{
			using(var dataContext = new DataContext())
			{
				var tournament = new Tournament
				{
					Key = Guid.NewGuid(),
				};

				dataContext.Tournaments.Add(tournament);
				dataContext.SaveChanges();

				return tournament;
			}
		}

		[Route("tournament/{key}")]
		public JObject Get(Guid key)
		{
			throw new NotImplementedException();
		}
    }
}
