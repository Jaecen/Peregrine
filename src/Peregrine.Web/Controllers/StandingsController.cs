using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	public class StandingsController : ApiController
	{
		readonly StandingsResponseProvider StandingsResponseProvider;

		public StandingsController(StandingsResponseProvider standingsResponseProvider)
		{
			if(standingsResponseProvider == null)
				throw new ArgumentException("standings");

			StandingsResponseProvider = standingsResponseProvider;
		}

		[Route("api/tournaments/{tournamentKey}/standings")]
		[Route("api/tournaments/{tournamentKey}/round/{roundNumber:min(1)}/standings")]
		public IHttpActionResult Get(Guid tournamentKey, int? roundNumber = null)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				if(roundNumber.HasValue && tournament.GetRound(roundNumber.Value) == null)
					return NotFound();

				return Ok(StandingsResponseProvider.Create(tournament, roundNumber));
			}
		}

		[Route("api/tournaments/{tournamentKey}/standings/updates")]
		[Route("api/tournaments/{tournamentKey}/round/{roundNumber:min(1)}/standings/updates")]
		public IHttpActionResult GetEventSource(Guid tournamentKey, int? roundNumber = null)
		{
			StandingsResponse initialState;
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				if(roundNumber.HasValue && tournament.GetRound(roundNumber.Value) == null)
					return NotFound();

				initialState = StandingsResponseProvider.Create(tournament, roundNumber);
			}

			return ResponseMessage(new HttpResponseMessage
			{
				Content = new PushStreamContent(
					(stream, content, context) =>
					{
						var streamWriter = new System.IO.StreamWriter(stream);

						EventStreamManager
							.PublishTo(streamWriter, "updated", initialState);

						EventStreamManager
							.GetInstance("standings", String.Format("{0}/{1}", tournamentKey, roundNumber))
							.AddListener(streamWriter);
					},
					"text/event-stream"
				),
			});
		}
	}
}