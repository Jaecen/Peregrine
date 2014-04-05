using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/rounds")]
	public class RoundController : ApiController
	{
		readonly EventPublisher EventPublisher;
		readonly TournamentManager TournamentManager;
		readonly RoundManager RoundManager;
		readonly RoundResponseProvider RoundResponseProvider;

		public RoundController(EventPublisher eventPublisher, TournamentManager tournamentManager, RoundManager roundManager, RoundResponseProvider roundResponseProvider)
		{
			if(eventPublisher == null)
				throw new ArgumentNullException("eventPublisher");

			if(tournamentManager == null)
				throw new ArgumentNullException("tournamentManager");

			if(roundManager == null)
				throw new ArgumentNullException("roundManager");

			if(roundResponseProvider == null)
				throw new ArgumentNullException("roundResponseProvider");

			EventPublisher = eventPublisher;
			TournamentManager = tournamentManager;
			RoundManager = roundManager;
			RoundResponseProvider = roundResponseProvider;
		}

		[Route("{roundNumber:min(1)}", Name="round-get")]
		public IHttpActionResult Get(Guid tournamentKey, int roundNumber)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var round = RoundManager.GetRound(tournament, roundNumber);

				if(round == null)
					return NotFound();

				return Ok(RoundResponseProvider.Create(tournament, round));
			}
		}

		[Route("{roundNumber:min(1)}/updates")]
		public IHttpActionResult GetEventSource(Guid tournamentKey, int roundNumber)
		{
			RoundResponse initialState;

			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var round = RoundManager.GetRound(tournament, roundNumber);

				if(round == null)
					return NotFound();

				initialState = RoundResponseProvider.Create(tournament, round);
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
								.GetInstance(String.Format("round/{0}/{1}", tournamentKey, roundNumber))
								.AddListener(streamWriter);
						},
					"text/event-stream"
				),
			});
		}

		[Route("current")]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var currentRoundNumber = TournamentManager.GetCurrentRoundNumber(tournament);

				if(currentRoundNumber == null)
					return NotFound();

				return RedirectToRoute(
						"round-get",
						new { tournamentKey = tournamentKey, roundNumber = currentRoundNumber.Value }
					);
			}
		}
	}
}