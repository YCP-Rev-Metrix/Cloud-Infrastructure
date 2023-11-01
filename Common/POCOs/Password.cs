namespace Common.POCOs;
public class Password : POCO
{
    public string RawPassword { set; get; }

    public Password(string rawPassword) => RawPassword = rawPassword;
}
