namespace Common.POCOs;
public class InsertUserData : POCO
{
    public InsertUserData(int userid, string firstname, string lastname, string username, string password, string email, string phone, string[] role )
    {
        Userid = userid;
        Firstname = firstname;
        Lastname = lastname;
        Username = username;
        Password = password;
        Email = email;
        Phone = phone;
        Role = role;
    }

    public int Userid { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string[] Role { get; set; }



}
