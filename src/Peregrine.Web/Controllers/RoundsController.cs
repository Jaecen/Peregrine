﻿using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;
using Peregrine.Web.Services;

namespace Peregrine.Web.Controllers
{
	[RoutePrefix("api/tournaments/{tournamentKey}/rounds")]
	public class RoundsController : ApiController
	{
		readonly RoundManager RoundManager;
		
		public RoundsController()
		{
			RoundManager = new RoundManager();
		}

		[Route]
		public IHttpActionResult Get(Guid tournamentKey)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.GetTournament(tournamentKey);

				if(tournament == null)
					return NotFound();

				var roundNumbers = tournament
					.Rounds
					.Select(round => new
						{
							number = round.Number
						})
					.OrderBy(round => round.number)
					.ToArray();

				var lastRoundNumber = tournament
					.Rounds
					.Select(round => round.Number)
					.OrderBy(number => number)
					.DefaultIfEmpty(0)
					.LastOrDefault();

				var nextRoundState = RoundManager.DetermineRoundState(tournament, lastRoundNumber + 1);

				if(nextRoundState != RoundState.Invalid && lastRoundNumber < RoundManager.GetMaxRoundsForTournament(tournament))
					roundNumbers = roundNumbers
						.Concat(new[] { new { number = lastRoundNumber + 1 }})
						.ToArray();
					
				return Ok(roundNumbers);
			}
		}
	}
}