using System.Linq;
using System.Web.Http.Routing;
using Peregrine.Data;
using Peregrine.Api.Model;

namespace Peregrine.Api.Services
{
	public class MatchRenderer
	{
		readonly EntityLinkRenderer EntityLinkRenderer;

		public MatchRenderer()
		{
			EntityLinkRenderer = new EntityLinkRenderer();
		}

		public object RenderSummary(Tournament tournament, Round round, Match match, UrlHelper url)
		{
			return new
			{
				number = match.Number,
				players = match.Players
					.OrderBy(player => player.Name)
					.Select(player => new
					{
						name = player.Name,
						_links = new[]
						{
							new EntityLink("self", url.Link("get-match", new { key = tournament.Key, roundNumber = round.Number, matchNumber = match.Number }))
						}
						.Select(el => EntityLinkRenderer.Render(el)),
					}),
				results = match.Games
					.OrderBy(game => game.Number)
					.Select(game => new
					{
						number = game.Number,
						winner = game.Winner != null ? game.Winner.Name : null,
					}),
				_links = new[]
					{
						new EntityLink("self", url.Link("get-match", new { key = tournament.Key, roundNumber = round.Number, matchNumber = match.Number })),
						new EntityLink("up", url.Link("get-round", new { key = tournament.Key, roundNumber = round.Number })),
					}
					.Select(el => EntityLinkRenderer.Render(el)),
			};
		}
	}
}