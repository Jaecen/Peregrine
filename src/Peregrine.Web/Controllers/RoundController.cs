using System;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/rounds")]
	public class RoundController : ApiController
	{
		readonly TournamentManager TournamentManager;
		readonly RoundManager RoundManager;
		readonly RoundResponseProvider RoundResponseProvider;

		public RoundController(TournamentManager tournamentManager, RoundManager roundManager, RoundResponseProvider roundResponseProvider)
		{
			if(tournamentManager == null)
				throw new ArgumentNullException("tournamentManager");

			if(roundManager == null)
				throw new ArgumentNullException("roundManager");

			if(roundResponseProvider == null)
				throw new ArgumentNullException("roundResponseProvider");

			TournamentManager = tournamentManager;
			RoundManager = roundManager;
			RoundResponseProvider = roundResponseProvider;
		}

		[Route("{roundNumber:min(1)}", Name = "round-get")]
		public IHttpActionResult Get(Guid tournamentKey, int roundNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var round = RoundManager.GetRound(tournament, roundNumber);

				if(round == null)
					return NotFound();

				return Ok(RoundResponseProvider.Create(tournament, round));
			}
		}
	}
}