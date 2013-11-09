using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/round/{roundNumber}")]
	public class RoundController : ApiController
	{
		readonly RoundManager RoundManager;

		public RoundController()
		{
			RoundManager = new RoundManager();
		}

		[Route(Name = "Round.Options")]
		public virtual IHttpActionResult Options()
		{
			return new ResourceActionResult(ControllerContext, Ok());
		}

		[Route(Name = "Round.Get")]
		public IHttpActionResult Get(Guid key, int roundNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				var round = tournament.GetRound(roundNumber);
				if(round == null)
					if(roundNumber == 1)
						round = RoundManager.GenerateFirstRound(tournament);
					else
						return NotFound();

				return Ok(this.RenderDetail(round, key));
			}
		}


	}
}
