using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/rounds")]
	public class RoundsController : ApiController
    {
		readonly RoundManager RoundManager;

		public RoundsController()
		{
			RoundManager = new RoundManager();
		}
	
		[Route(Name = "Rounds.Options")]
		public virtual IHttpActionResult Options()
		{
			return new ResourceActionResult(ControllerContext, Ok());
		}
	
		[Route(Name = "Rounds.Get")]
		public IHttpActionResult GetList(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				return Ok(
					this.RenderDetail(
						tournament
							.Rounds
							.DefaultIfEmpty(RoundManager.GenerateFirstRound(tournament)),
						key
					)
				);
			}
		}
    }
}
