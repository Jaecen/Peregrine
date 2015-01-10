using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/rounds/active")]
	public class ActiveRoundController : ApiController
	{
		readonly ActiveRoundResponseProvider ActiveRoundResponseProvider;

		public ActiveRoundController(ActiveRoundResponseProvider activeRoundResponseProvider)
		{
			if(activeRoundResponseProvider == null)
				throw new ArgumentNullException("activeRoundResponseProvider");

			ActiveRoundResponseProvider = activeRoundResponseProvider;
		}

		[Route]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				if(tournament.ActiveRoundNumber == null)
					return NotFound();

				return Ok(ActiveRoundResponseProvider.Create(tournament, tournament.ActiveRoundNumber));
			}
		}

		[Route]
		[Authorize]
		public IHttpActionResult Put(Guid tournamentKey, [FromBody] ActiveRoundRequest request)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				tournament.ActiveRoundNumber = request.roundNumber;
				dataContext.SaveChanges();

				return Ok(ActiveRoundResponseProvider.Create(tournament, tournament.ActiveRoundNumber));
			}
		}
	}
}