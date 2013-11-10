using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peregrine.Api.Model
{
	public class EntityLink
	{
		public readonly string Rel;
		public readonly string Href;

		public EntityLink(string rel, string href)
		{
			Rel = rel;
			Href = href;
		}
	}
}