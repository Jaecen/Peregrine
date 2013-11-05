using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}")]
    public class PlayerController : ApiController
    {
		[Route("players", Name = "Player.List")]
		public IHttpActionResult Get(Guid key)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.FirstOrDefault(t => t.Key == key);

				if(tournament == null)
					return NotFound();

				return Ok(new
				{
					players = tournament
						.Players
						.OrderBy(p => p.Name)
						.Select(p => new
						{
							_link = Url.Link("Player.Get", new { key = tournament.Key, name = p.Name }),
							name = p.Name,
						}),
				});
			}
		}
		
		[Route("player/{name}", Name = "Player.Post")]
		public IHttpActionResult Put(Guid key, string name)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.FirstOrDefault(t => t.Key == key);

				if(tournament == null)
					return NotFound();

				if(tournament
					.Players
					.Where(p => p.Name == name)
					.Any())
					return Conflict();

				var player = new Player
				{
					Name = name,
				};

				tournament.Players.Add(player);
				dataContext.SaveChanges();

				return CreatedAtRoute("Player.Get", new { key = tournament.Key, name = player.Name }, new
				{
					_link = Url.Link("Player.Get", new { key = tournament.Key, name = player.Name }),
					name = player.Name,
				});
			}
		}

		[Route("player/{name}", Name = "Player.Get")]
		public IHttpActionResult Get(Guid key, string name)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.FirstOrDefault(t => t.Key == key);

				if(tournament == null)
					return NotFound();

				var player = tournament
					.Players
					.Where(p => p.Name == name)
					.FirstOrDefault();

				if(player == null)
					return NotFound();

				return Ok(new
				{
					_link = Url.Link("Player.Get", new { key = tournament.Key, name = player.Name }),
					name = player.Name,
				});
			}
		}

		[Route("player/{name}", Name = "Player.Delete")]
		public IHttpActionResult Delete(Guid key, string name)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext
					.Tournaments
					.FirstOrDefault(t => t.Key == key);

				if(tournament == null)
					return NotFound();

				var player = tournament
					.Players
					.Where(p => p.Name == name)
					.FirstOrDefault();

				if(player == null)
					return NotFound();

				//TODO: Implement drop vs. delete

				tournament.Players.Remove(player);
				dataContext.SaveChanges();

				return Ok();
			}
		}
    }
}
