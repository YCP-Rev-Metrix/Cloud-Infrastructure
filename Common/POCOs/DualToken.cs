namespace Common.POCOs;
public class DualToken : POCO
{
    public string TokenA { get; set; } // Typically Authorization Token
    public byte[] TokenB { get; set; } // Typically Refresh Token

    public DualToken(string tokenA, byte[] tokenB)
    {
        TokenA = tokenA;
        TokenB = tokenB;
    }
}
