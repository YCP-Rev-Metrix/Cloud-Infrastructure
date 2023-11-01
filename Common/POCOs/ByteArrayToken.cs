namespace Common.POCOs;
public class ByteArrayToken : POCO
{
    public ByteArrayToken(byte[] token) => Token = token;

    public byte[] Token { get; set; }
}
