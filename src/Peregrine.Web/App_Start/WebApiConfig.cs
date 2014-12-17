using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Peregrine.Data;
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

			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

			builder.RegisterType<RoundManager>();
			builder.RegisterType<TournamentManager>();
			builder.RegisterType<StatsProvider>();

			builder.RegisterType<TournamentResponseProvider>();
			builder.RegisterType<PlayerResponseProvider>();
			builder.RegisterType<RoundResponseProvider>();
			builder.RegisterType<MatchResponseProvider>();
			builder.RegisterType<PlayerMatchStatsResponseProvider>();
			builder.RegisterType<StandingsResponseProvider>();
			builder.RegisterType<ActiveRoundResponseProvider>();

			#region Identity

			builder
				.Register(c => new IdentityDataContext())
				.AsSelf();

			builder
				.Register(c => new UserStore<User>(c.Resolve<IdentityDataContext>()))
				.As<IUserStore<User>>()
				.InstancePerRequest();

			builder
				.Register(c => new IdentityFactoryOptions<ApplicationUserManager>())
				.AsSelf()
				.InstancePerRequest();

			builder
				.Register(c => new ApplicationUserManager(c.Resolve<IUserStore<User>>(), new IdentityFactoryOptions<ApplicationUserManager>()))
				.As<UserManager<User>>();

			builder
				.Register(c => new ExternalLoginContextProvider())
				.AsSelf()
				.SingleInstance();

			#endregion

			return builder;
		}
	}
}
