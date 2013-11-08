using System;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/round/{roundNumber}/match/{matchNumber}")]
	public class MatchController : ApiController
    {
		public enum MatchResult
		{
			Win,
			Draw,
		};

		[Route(Name = "Match.Options")]
		public virtual IHttpActionResult Options()
		{
			return new ResourceActionResult(ControllerContext, Ok());
		}

		[Route(Name = "Match.Get")]
		public IHttpActionResult Get(Guid key, int roundNumber, int matchNumber)
		{
			using(var dataContext = new DataContext())
			{
				var match = dataContext
					.GetTournament(key)
					.GetRound(roundNumber)
					.GetMatch(matchNumber);

				if(match == null)
					return NotFound();

				return Ok(this.RenderDetail(match, key, roundNumber));
			}
		}

		[Route(Name = "MatchResult.Put")]
		public IHttpActionResult Put(Guid key, int roundNumber, string name, MatchResult result)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				var player = tournament.GetPlayer(name);
				var match = tournament
					.GetRound(roundNumber)
					.GetMatch(player);

				if(match == null)
					return NotFound();

				// Null winner indicates draw
				match.Games.Add(new Game
				{
					Number = match.Games.Count + 1,
					Winner = result == MatchResult.Win ? player : null,
				});

				dataContext.SaveChanges();

				return Ok(this.RenderDetail(match, key, roundNumber));
			}
		}
    }
}
