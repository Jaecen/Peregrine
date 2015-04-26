using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Peregrine.Data
{

    public class AuthRepository : IDisposable
    {
        private DataContext DataContext;

        private UserManager<IdentityUser> UserManager;

        public AuthRepository()
        {
			DataContext = new DataContext();
			UserManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(DataContext));
        }

        public async Task<IdentityResult> RegisterUser(User userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName
            };

			var result = await UserManager.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
			IdentityUser user = await UserManager.FindAsync(userName, password);

            return user;
        }

        public Client FindClient(string clientId)
        {
			var client = DataContext.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

           var existingToken = DataContext.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

           if (existingToken != null)
           {
             var result = await RemoveRefreshToken(existingToken);
           }
          
            DataContext.RefreshTokens.Add(token);

            return await DataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
           var refreshToken = await DataContext.RefreshTokens.FindAsync(refreshTokenId);

           if (refreshToken != null) {
               DataContext.RefreshTokens.Remove(refreshToken);
               return await DataContext.SaveChangesAsync() > 0;
           }

           return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            DataContext.RefreshTokens.Remove(refreshToken);
             return await DataContext.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await DataContext.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
             return  DataContext.RefreshTokens.ToList();
        }

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            IdentityUser user = await UserManager.FindAsync(loginInfo);

            return user;
        }

		public IdentityUser GetUser(UserLoginInfo loginInfo)
		{
			IdentityUser user = UserManager.Find(loginInfo);
			return user;
		}

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            var result = await UserManager.CreateAsync(user);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await UserManager.AddLoginAsync(userId, login);

            return result;
        }

        public void Dispose()
        {
            DataContext.Dispose();
            UserManager.Dispose();
        }
    }
}