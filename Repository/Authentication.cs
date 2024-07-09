using Microsoft.Extensions.Caching.Memory;

namespace ServerApp.Repository;
public class Authentication
{
    Datacontext datacontext;
    public Authentication(Datacontext datacontext)
    {
        this.datacontext = datacontext;
    }

    public async Task<User> LoginAsync(string username, string password) {

        var user =datacontext.Users.FirstOrDefault(
            x => x.UserName==username 
            && x.Password==password);
        if (user != null) return new User() { Id = user.Id,UserName=user.UserName};
        return new User();
    }
    public async Task<User> RegisterUser(string userName, string password)
    {
        var dser= datacontext.AddUser(userName, password);
        return new User() { Id=dser.Id,UserName=userName}; 
    }

}
public record User {
    public int Id;
    public string UserName="";
};
