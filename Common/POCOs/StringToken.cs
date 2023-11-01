namespace Common.POCOs;
public class StringToken : POCO
{
    public StringToken(string token) => Token = token;

    public string Token { get; set; }
}
