using Peregrine.Api.Model;

namespace Peregrine.Api.Services
{
	public class EntityLinkRenderer
	{
		public object Render(EntityLink entityLink)
		{
			return new
			{
				rel = entityLink.Rel,
				href = entityLink.Href,
			};
		}
	}
}