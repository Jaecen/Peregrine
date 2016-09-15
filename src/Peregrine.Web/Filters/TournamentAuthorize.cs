using Peregrine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Security.Claims;

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
			var user = actionContext.RequestContext.Principal;

			if(tournamentKey == null || user == null)
				throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);

			if(user.IsInRole("Admin"))
				return;

			using(var dataContext = new DataContext())
			{
				var userOwnsTournament = dataContext
					.Users
					.Where(u => u.UserName == user.Identity.Name)
					.FirstOrDefault()
					?.OrganizedTournaments
					.Select(t => t.Key.ToString())
					.Contains(tournamentKey.ToString(), StringComparer.OrdinalIgnoreCase)
					?? false;

				if(!userOwnsTournament)
					throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
			}
		}

	}
}