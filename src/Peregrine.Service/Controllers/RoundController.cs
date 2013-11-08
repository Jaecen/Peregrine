using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/round/{roundNumber}")]
	public class RoundController : ApiController
    {
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
						round = GenerateFirstRound(tournament);
					else
						return NotFound();

				return Ok(this.RenderDetail(round, key));
			}
		}

		private Round GenerateFirstRound(Tournament tournament)
		{
			// Generate initial order by hashing tournament key + name
			return new Round
			{
				Number = 1,
				Matches = tournament
					.Players
					.OrderBy(p => String.Format("{0}{1}{2}", tournament.Key, p.Name, tournament.Players.Count).ComputeMd5())
					.PartitionBy(2)
					.Select((partition, index) => new Match
					{
						Number = index + 1,
						Players = partition.ToArray(),
					})
					.ToArray()
			};
		}
    }
}
