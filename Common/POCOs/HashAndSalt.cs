namespace Common.POCOs;
public class HashAndSalt : POCO
{
    public byte[] Hash { set; get; }
    public byte[] Salt { set; get; }

    public HashAndSalt(byte[] hash, byte[] salt)
    {
        Hash = hash;
        Salt = salt;
    }
}
