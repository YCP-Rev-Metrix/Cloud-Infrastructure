namespace Common.POCOs;
public class InsertUserData : POCO
{
    public InsertUserData(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; set; }
    public string Password { get; set; }
}
