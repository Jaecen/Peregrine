using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/rounds")]
	public class RoundsController : ApiController
	{
		readonly RoundManager RoundManager;
		
		public RoundsController()
		{
			RoundManager = new RoundManager();
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

				var roundNumbers = tournament
					.Rounds
					.Select(round => round.Number)
					.OrderBy(number => number)
					.ToArray();

				var lastRoundNumber = tournament
					.Rounds
					.OrderBy(number => number)
					.Select(round => round.Number)
					.DefaultIfEmpty(0)
					.LastOrDefault();

				var nextRoundState = RoundManager.DetermineRoundState(tournament, lastRoundNumber + 1);

				if(nextRoundState != RoundState.Invalid)
					roundNumbers = roundNumbers
						.Concat(new[] { lastRoundNumber + 1 })
						.ToArray();
					
				return Ok(roundNumbers);
			}
		}
	}
}