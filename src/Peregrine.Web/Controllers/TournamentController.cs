using System;
using System.Net;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Filters;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}")]
	public class TournamentController : ApiController
	{
		readonly TournamentResponseProvider TournamentResponseProvider;

		public TournamentController(TournamentResponseProvider tournamentResponseProvider)
		{
			if(tournamentResponseProvider == null)
				throw new ArgumentNullException("tournamentResponseProvider");

			TournamentResponseProvider = tournamentResponseProvider;
		}

		[Route(Name = "tournament-get")]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				return Ok(TournamentResponseProvider.Create(tournament));
			}
		}

		[Route]
		[Authorize]
		[TournamentAuthorize]
		public IHttpActionResult Put(Guid tournamentKey, [FromBody] TournamentRequest requestBody)
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

				return Ok(TournamentResponseProvider.Create(tournament));
			}
		}

		[Route]
		[Authorize]
		[TournamentAuthorize]
		public IHttpActionResult Delete(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				dataContext.Tournaments.Remove(tournament);
				dataContext.SaveChanges();

				return StatusCode(HttpStatusCode.NoContent);
			}
		}
	}
}
