using Peregrine.Service.Model;

namespace Peregrine.Service.Services
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