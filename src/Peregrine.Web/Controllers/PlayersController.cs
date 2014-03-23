using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/players")]
	public class PlayersController : ApiController
	{
		[Route]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var playerNames = tournament
					.Players
					.Select(player => new
						{
							name = player.Name
						})
					.ToArray();

				return Ok(playerNames);
			}
		}
	}
}