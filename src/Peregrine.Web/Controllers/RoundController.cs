using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/rounds/{roundNumber}")]
	public class RoundController : ApiController
	{
		readonly RoundManager RoundManager;

		public RoundController()
		{
			RoundManager = new RoundManager();
		}

		[Route]
		public IHttpActionResult Get(Guid tournamentKey, int roundNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
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

				return Ok(new
					{
						number = round.Number,
						completed = roundState == RoundState.Completed || roundState == RoundState.Finalized,
						matches = round
							.Matches
							.Select(m => new
							{
								games = m.Games.Count(),
								players = m.Players
									.Select(p => new
									{
										name = p.Name,
										wins = m.Games.Where(g => g.Winner == p).Count(),
										losses = m.Games.Where(g => g.Winner != p && g.Winner != null).Count(),
										draws = m.Games.Where(g => g.Winner == null).Count(),
									})
							})
					});
			}
		}
	}
}