using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/rounds")]
	public class RoundController : ApiController
	{
		readonly RoundManager RoundManager;

		public RoundController()
		{
			RoundManager = new RoundManager();
		}

		[Route("{roundNumber:min(1)}")]
		public IHttpActionResult Get(Guid tournamentKey, int roundNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				if(roundNumber > RoundManager.GetMaxRoundsForTournament(tournament))
					return NotFound();

				var roundState = RoundManager.DetermineRoundState(tournament, roundNumber);

				if(roundState == RoundState.Invalid)
					return NotFound();

				Round round;
				if(roundState == RoundState.Projected)
					round = new Round
					{
						Number = roundNumber,
						Matches = RoundManager.CreateMatches(tournament, roundNumber),
					};
				else
					round = tournament
						.Rounds
						.Where(r => r.Number == roundNumber)
						.FirstOrDefault();

				return Ok(RoundManager.RenderRound(round, roundState));
			}
		}

		[Route("current")]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			int currentRoundNumber;
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var roundStatus = Enumerable
					.Range(1, RoundManager.GetMaxRoundsForTournament(tournament))
					.Select(roundNumber => new
						{
							Number = roundNumber,
							State = RoundManager.DetermineRoundState(tournament, roundNumber)
						});

				if(!roundStatus.Any())
					return NotFound();

				var currentRound = roundStatus
					.SkipWhile(o => o.State >= RoundState.Final)
					.FirstOrDefault();

				if(currentRound == null || currentRound.State == RoundState.Invalid)
					return NotFound();

				currentRoundNumber = currentRound.Number;
			}

			return Get(tournamentKey, currentRoundNumber);
		}
	}
}