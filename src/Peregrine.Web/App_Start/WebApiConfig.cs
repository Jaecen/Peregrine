using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Peregrine.Web.Models;
using Peregrine.Web.Services;

namespace Peregrine.Web
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
			
			var builder = CreateBuilder();

			var container = builder.Build();
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
		}

		static ContainerBuilder CreateBuilder()
		{
			var builder = new ContainerBuilder();

			builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());

			builder.RegisterType<EventPublisher>();
			builder.RegisterType<RoundManager>();
			builder.RegisterType<TournamentManager>();
	
			builder.RegisterType<TournamentResponseProvider>();
			builder.RegisterType<PlayerResponseProvider>();
			builder.RegisterType<RoundResponseProvider>();
			builder.RegisterType<MatchResponseProvider>();
			builder.RegisterType<PlayerMatchStatsResponseProvider>();

			return builder;
		}
	}
}
