using Microsoft.Extensions.Caching.Memory;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace ServerApp.Repository
{
    public class Datacontext
    {
        IMemoryCache memoryCache;
        IHostEnvironment hostEnvironment;
        public List<DbUser> Users
        {
            get
            {
                if (!memoryCache.TryGetValue("users", out List<DbUser>? cacheUsers))
                {
                    return LoadUsers();
                }
                return cacheUsers!;
            }
        }
        public Datacontext(IMemoryCache memoryCache, IHostEnvironment hostEnvironment)
        {
            this.memoryCache = memoryCache;
            this.hostEnvironment = hostEnvironment;
        }

        public DbUser AddUser(string username, string password)
        {
            DbUser? existingUser = Users.FirstOrDefault(
                x => x.UserName == username && x.Password == password);
            if (existingUser != null)
            {
                return existingUser;
            }
            else
            {
                DbUser newUser = new()
                {
                    Id = Users.Count + 1,
                    UserName = username,
                    Password = password
                };
                Users.Add(newUser);
                string jsonString = JsonSerializer.Serialize(Users);
                var path = hostEnvironment.ContentRootPath + "\\data\\users.json";
                TextWriter writer = new StreamWriter(path, false);
                writer.Write(jsonString);
                writer.Close();
                memoryCache.Set<List<DbUser>>("users", Users);

            }
            return new DbUser() { Id = -1 };
        }

        private List<DbUser> LoadUsers()
        {
            var path = hostEnvironment.ContentRootPath + "\\data\\users.json";
            if (File.Exists(path))
            {
                TextReader reader = new StreamReader(path, false);
                var fileContents = reader.ReadToEnd();
                var users = JsonSerializer.Deserialize<List<DbUser>>(fileContents);
                this.memoryCache.Set<List<DbUser>>("users", users);
                reader.Close();
                return users;
            }
            this.memoryCache.Set<List<DbUser>>("users", new List<DbUser>());
            return new List<DbUser>();



        }
    }
    public class DbUser()
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
