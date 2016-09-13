using Peregrine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace Peregrine.Web.Filters
{
	public class TournamentAuthorize : ActionFilterAttribute	
	{
		public readonly string TournamentKeyName;

		public TournamentAuthorize(string tournamentKeyName = "tournamentKey")
		{
			TournamentKeyName = tournamentKeyName;
		}

		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			var tournamentKey = actionContext.ActionArguments[TournamentKeyName];
			var identity = actionContext.RequestContext.Principal.Identity;

			if(tournamentKey == null || identity == null)
				throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

			using(var dataContext = new DataContext())
			{
				var userOwnsTournament = dataContext
					.Users
					.Where(u => u.UserName == identity.Name)
					.FirstOrDefault()
					?.OrganizedTournaments
					.Select(t => t.Key.ToString())
					.Contains(tournamentKey.ToString(), StringComparer.OrdinalIgnoreCase)
					?? false;

				if(!userOwnsTournament)
					throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
			}

			base.OnActionExecuting(actionContext);
		}

	}
}