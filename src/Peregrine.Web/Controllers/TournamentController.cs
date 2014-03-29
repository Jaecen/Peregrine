using System;
using System.Net;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}")]
	public class TournamentController : ApiController
	{
		[Route(Name = "tournament-get")]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				return Ok(new TournamentResponseBody(tournament));
			}
		}

		[Route]
		public IHttpActionResult Put(Guid tournamentKey, [FromBody] TournamentRequestBody requestBody)
		{
			if(requestBody == null)
				return BadRequest("No request body provided.");

			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				tournament.Name = requestBody.name;
				dataContext.SaveChanges();

				return Ok(new TournamentResponseBody(tournament));
			}
		}

		[Route]
		public IHttpActionResult Delete(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				return StatusCode(HttpStatusCode.NoContent);
			}
		}
	}
}
