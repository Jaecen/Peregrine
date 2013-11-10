using System.Linq;
using System.Web.Http.Routing;
using Peregrine.Data;
using Peregrine.Service.Model;

namespace Peregrine.Service.Services
{
	public class RoundRenderer
	{
		readonly EntityLinkRenderer EntityLinkRenderer;
		
		public RoundRenderer()
		{
			EntityLinkRenderer = new EntityLinkRenderer();
		}

		public object RenderSummary(Tournament tournament, Round round, UrlHelper url)
		{
			return new
			{
				number = round.Number,
				_links = new[]
					{
						new EntityLink("self", url.Link("get-round", new { key = tournament.Key, roundNumber = round.Number })),
						new EntityLink("up", url.Link("list-rounds", new { key = tournament.Key })),
						round.Number > 1 ? new EntityLink("prev", url.Link("get-round", new { key = tournament.Key, roundNumber = round.Number - 1 })) : null,
						round.Number < tournament.Rounds.Count ? new EntityLink("next", url.Link("get-round", new { key = tournament.Key, roundNumber = round.Number + 1 })) : null,
						new EntityLink("matches", url.Link("list-matches", new { key = tournament.Key, roundNumber = round.Number })),
					}
					.Where(el => el != null)
					.Select(el => EntityLinkRenderer.Render(el)),
			};
		}
	}
}