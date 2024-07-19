using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
public partial class Repository
{
    public User LoginUser(string username, string password)
    {
        StringEncryptor cryptor = new("ThisIsASuperSecretKeyPituusMin32", "ThisIsASuperSecretIV");

        string passwordHash = cryptor.Encrypt(username.ToUpper() + password);
        User user = new User();
        SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();

        string sql = "SELECT Id,Username,Email,FirstName,Lastname " +
            "FROM dbo.Users " +
            "Where PasswordHash=@passwordHash ";
        SqlCommand command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("PasswordHash", passwordHash);

        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (user.Id > 0)
            {
                return new User();
            }
            user.Id = Convert.ToInt32(reader["Id"]);
            user.UserName = reader["UserName"].ToString();
            user.Email = reader["Email"].ToString();
            user.FirstName = reader["FirstName"].ToString();
            user.LastName = reader["LastName"].ToString();
        }
        return user;
    }

    public (User user,string message) RegisterUser(User newUser, string password)
    {
        User user = new();
        if (string.IsNullOrEmpty(newUser.UserName)) return (user,"uesername empty");
        User existingUser = GetUser(newUser.UserName);
        if (existingUser.Id > 0) return (user,"username already in use");

        SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();

        string sql = "INSERT INTO Users (Username,PasswordHash,Email,FirstName,Lastname) " +
            "VALUES(@Username,@PasswordHash,@Email,@FirstName,@Lastname) ";
        SqlCommand command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("Username", newUser.UserName);

        StringEncryptor cryptor = new("ThisIsASuperSecretKeyPituusMin32", "ThisIsASuperSecretIV");
        string passwordHash = cryptor.Encrypt(newUser.UserName!.ToUpper() + password);
        command.Parameters.AddWithValue("PasswordHash", passwordHash);

        command.Parameters.AddWithValue("Email", newUser.Email != null ? newUser.Email : DBNull.Value);
        command.Parameters.AddWithValue("FirstName", newUser.FirstName != null ? newUser.FirstName : DBNull.Value);
        command.Parameters.AddWithValue("Lastname", newUser.LastName != null ? newUser.LastName : DBNull.Value);

        int result = command.ExecuteNonQuery();
        if (result != 1) return (user,"error in save nuw user");
        return (LoginUser(newUser.UserName!, password),"");

    }
    public User GetUser(string username)
    {
        User user = new User();
        SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();

        string sql = "SELECT Id,Username,Email,FirstName,Lastname " +
            "FROM dbo.Users " +
            "Where UserName=@UserName ";
        SqlCommand command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("UserName", username);

        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (user.Id > 0)
            {
                return new User();
            }
            user.Id = Convert.ToInt32(reader["Id"]);
            user.UserName = reader["UserName"].ToString();
            user.Email = reader["Email"].ToString();
            user.FirstName = reader["FirstName"].ToString();
            user.LastName = reader["LastName"].ToString();
        }
        return user;
    }

}
public class User()
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public static partial class Validators 
{
    public static IEnumerable<(string field,string error)> Validate(this User user)
    {
        List<(string field, string error)> errors= new();
        errors.Add(new ("UserName", user.UserName.Verify(false)));
        errors.Add(new("Email", user.Email.Verify(false)));
        errors.Add(new("FirstName", user.FirstName.Verify(false)));
        errors.Add(new("LastName", user.LastName.Verify(false)));
        return errors.Where(x=>x.error!="");

    }
}