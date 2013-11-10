using System.Linq;
using System.Web.Http.Routing;
using Peregrine.Data;
using Peregrine.Service.Model;

namespace Peregrine.Service.Services
{
	public class TournamentRenderer
	{
		readonly EntityLinkRenderer EntityLinkRenderer;
		
		public TournamentRenderer()
		{
			EntityLinkRenderer = new EntityLinkRenderer();
		}

		public object RenderSummary(Tournament tournament, UrlHelper url)
		{
			return new
			{
				key = tournament.Key,
				_links = new[]
					{
						new EntityLink("self", url.Link("get-tournament", new { key = tournament.Key })),
						new EntityLink("players", url.Link("list-players", new { key = tournament.Key })),
						new EntityLink("rounds", url.Link("list-rounds", new { key = tournament.Key })),
					}
					.Select(el => EntityLinkRenderer.Render(el)),
			};
		}
	}
}