using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Caching.Memory;
using ServerApp.Repository;
using System.Net.Http;

namespace ServerApp.Services
{
    public class Session
    {
        private readonly Authentication authentication;
        public readonly Guid SessionToken = Guid.NewGuid();
        public User User { get; private set;} =new User();
        public Session(Authentication authentication)
        {
            this.authentication = authentication;
        }
        public void LogOut()
        {
            User = new();
        }

        public async Task<bool> Login(string userName, string password)
        {
            User = await authentication.LoginAsync(userName, password);
            return User.Id>0;
        }
        public async Task<bool> RegisterUser(string userName, string password)
        {
            User = await authentication.RegisterUser(userName, password);
            return User.Id > 0;
        }
    }

}
