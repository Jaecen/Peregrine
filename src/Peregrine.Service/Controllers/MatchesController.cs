using System;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/round/{roundNumber}/matches")]
	public class MatchesController : ApiController
    {
		[Route(Name = "Matches.Options")]
		public virtual IHttpActionResult Options()
		{
			return new ResourceActionResult(ControllerContext, Ok());
		}
		
		[Route(Name = "Matches.Get")]
		public IHttpActionResult GetList(Guid key, int roundNumber)
		{
			using(var dataContext = new DataContext())
			{
				var round = dataContext
					.GetTournament(key)
					.GetRound(roundNumber);

				if(round == null)
					return NotFound();

				return Ok(this.RenderDetail(round.Matches, key, roundNumber));
			}
		}
    }
}
