using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peregrine.Service.Model
{
	public class ActionLink
	{
		public readonly string Name;
		public readonly string Method;
		public readonly string Href;

		public ActionLink(string name, string method, string href)
		{
			Name = name;
			Method = method;
			Href = href;
		}
	}
}