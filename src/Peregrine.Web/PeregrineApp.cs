using System;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Peregrine.Data;
using Peregrine.Web.Models;
using Peregrine.Web.Providers;
using Peregrine.Web.Services;
using System.Web.Configuration;

#pragma warning disable 1998

[assembly: OwinStartup(typeof(Peregrine.Web.PeregrineApp))]
namespace Peregrine.Web
{
	public class PeregrineApp
	{
		public const string OAuthBearerAuthenticationType = OAuthDefaults.AuthenticationType;
		public const string PublicClientId = "self";

		public void Configuration(IAppBuilder app)
		{
			var builder = RegisterComponents(app);
			var container = builder.Build();

			var config = new HttpConfiguration();
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
			config.MapHttpAttributeRoutes();

			app.UseAutofacMiddleware(container);
			app.UseAutofacWebApi(config);

			// Enable the application to use a cookie to store information for the signed in user
			// and to use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive,
			});

			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			// Enable the application to use bearer tokens to authenticate users
			app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
			{
				AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
				Provider = new OAuthBearerAuthenticationProvider
				{
					OnValidateIdentity = async c => { c.Validated(); }
				},
				AccessTokenFormat = container.Resolve<ISecureDataFormat<AuthenticationTicket>>(),
			});

			app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
			{
				AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive,
				TokenEndpointPath = new PathString("/Token"),
				Provider = new ApplicationOAuthProvider(PublicClientId, container.Resolve<Func<ApplicationUserManager>>()),
				AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
				AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
				AllowInsecureHttp = true,
				AccessTokenFormat = container.Resolve<ISecureDataFormat<AuthenticationTicket>>(),
			});

			// Uncomment the following lines to enable logging in with third party login providers
			//app.UseMicrosoftAccountAuthentication(
			//    clientId: "",
			//    clientSecret: "");

			//app.UseTwitterAuthentication(
			//    consumerKey: "",
			//    consumerSecret: "");

			app.UseFacebookAuthentication(new FacebookAuthenticationOptions()
			{
				AppId = WebConfigurationManager.AppSettings["FacebookAppId"],
				AppSecret = WebConfigurationManager.AppSettings["FacebookAppSecret"],
				Provider = new FacebookAuthProvider()
			});

			app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
			{
				ClientId = WebConfigurationManager.AppSettings["GoogleClientId"],
				ClientSecret = WebConfigurationManager.AppSettings["GoogleClientSecret"],
				Provider = new GoogleAuthProvider()
			});

			app.UseWebApi(config);
		}

		ContainerBuilder RegisterComponents(IAppBuilder app)
		{
			var builder = new ContainerBuilder();

			builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());

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

			builder
				.Register(c => new DataContext())
				.AsSelf();

			builder
				.Register(c => new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(c.Resolve<DataContext>()))
				.As<IUserStore<ApplicationUser>>();

			builder
				.Register(c => new ApplicationUserManager(
					store: c.Resolve<IUserStore<ApplicationUser>>(),
					dataProtector: c.Resolve<IDataProtector>()))
				.AsSelf();

			builder
				.Register(c => new DpapiDataProtectionProvider().Create())
				.As<IDataProtector>();

			// These need to be explicitly named, otherwise auth tickets won't decrypt between instances
			var dataProtectionProvider = app.GetDataProtectionProvider();
			var authTicketDataProtector = dataProtectionProvider.Create("Auth Ticket");

			builder
				.RegisterInstance(authTicketDataProtector)
				.Named<IDataProtector>("Auth Ticket");

			builder
				.Register(c => new TicketDataFormat(c.ResolveNamed<IDataProtector>("Auth Ticket")))
				.As<ISecureDataFormat<AuthenticationTicket>>();

			return builder;
		}
	}
}