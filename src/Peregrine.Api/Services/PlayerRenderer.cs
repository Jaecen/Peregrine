using System.Linq;
using System.Web.Http.Routing;
using Peregrine.Data;
using Peregrine.Api.Model;

namespace Peregrine.Api.Services
{
	public class PlayerRenderer
	{
		readonly EntityLinkRenderer EntityLinkRenderer;

		public PlayerRenderer()
		{
			EntityLinkRenderer = new EntityLinkRenderer();
		}

		public object RenderSummary(Tournament tournament, Player player, UrlHelper url)
		{
			return new
			{
				name = player.Name,
				_links = new[]
					{
						new EntityLink("self", url.Link("get-player", new { key = tournament.Key, name = player.Name })),
						new EntityLink("up", url.Link("list-players", new { key = tournament.Key })),
					}
					.Select(el => EntityLinkRenderer.Render(el)),
			};
		}
	}
}