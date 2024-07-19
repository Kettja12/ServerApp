using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Caching.Memory;
using ServerApp;
using System.Net.Http;

namespace ServerApp.Services
{
    public class Session
    {
        Repository repository;
        public readonly Guid SessionToken = Guid.NewGuid();
        public User User { get; private set;} =new User();
        public Session(Repository repository)
        {
            this.repository = repository;
        }
        public void LogOut()
        {
            User = new();
        }

        public async Task<bool> Login(string userName, string password)
        {
            User = repository.LoginUser(userName, password);
            return User.Id>0;
        }
        public async Task<(bool status,string message)> RegisterUser(User user, string password)
        {
            string message;
            (User,message) = repository.RegisterUser(user, password);
            return (User.Id > 0,message);
        }
    }

}
