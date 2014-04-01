using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/players")]
	public class PlayersController : ApiController
	{
		readonly PlayerResponseProvider PlayerResponseProvider;

		public PlayersController(PlayerResponseProvider playerResponseProvider)
		{
			if(playerResponseProvider == null)
				throw new ArgumentNullException("playerResponseProvider");

			PlayerResponseProvider = playerResponseProvider;
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

				var playerNames = tournament
					.Players
					.Select(player => PlayerResponseProvider.Create(player))
					.ToArray();

				return Ok(playerNames);
			}
		}
	}
}