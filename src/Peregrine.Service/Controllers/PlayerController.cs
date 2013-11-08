using System;
using System.Linq;
using System.Web.Http;
using Peregrine.Data;

namespace Peregrine.Service.Controllers
{
	[RoutePrefix("tournament/{key}/player/{name}")]
    public class PlayerController : ApiController
    {
		[Route(Name = "Player.Options")]
		public virtual IHttpActionResult Options()
		{
			return new ResourceActionResult(ControllerContext, Ok());
		}
		
		[Route(Name = "Player.Post")]
		public IHttpActionResult Put(Guid key, string name)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
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

		[Route(Name = "Player.Get")]
		public IHttpActionResult Get(Guid key, string name)
		{
			using(var dataContext = new DataContext())
			{
				var player = dataContext
					.GetTournament(key)
					.GetPlayer(name);

				if(player == null)
					return NotFound();

				return Ok(this.RenderDetail(player, key));
			}
		}

		[Route(Name = "Player.Delete")]
		public IHttpActionResult Delete(Guid key, string name)
		{
			using(var dataContext = new DataContext())
			{
				var tournament = dataContext.GetTournament(key);
				if(tournament == null)
					return NotFound();

				var player = tournament.GetPlayer(name);
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
