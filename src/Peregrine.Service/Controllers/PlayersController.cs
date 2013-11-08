using System;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/players")]
	public class PlayersController : ApiController
	{
		[Route(Name = "Players.Options")]
		public virtual IHttpActionResult Options()
		{
			return new ResourceActionResult(ControllerContext, Ok());
		}

		[Route(Name = "Players.Get")]
		public IHttpActionResult Get(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				return Ok(this.RenderDetail(tournament.Players, key));
			}
		}
	}
}
