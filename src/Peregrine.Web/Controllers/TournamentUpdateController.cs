using System;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/updates")]
	public class TournamentUpdateController : ApiController
	{
		[Route]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			return ResponseMessage(new HttpResponseMessage
			{
				Content = new PushStreamContent(
					(stream, content, context) => EventStreamManager.GetInstance(tournamentKey).AddListener(new StreamWriter(stream)),
					"text/event-stream"
				),
			});
		}
	}
}