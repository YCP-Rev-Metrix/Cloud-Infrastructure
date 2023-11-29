namespace DatabaseCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var userDB = new UserDB();//ResearchDB ResearchDB = new ResearchDB();
        var researchDB = new ResearchDB();

        _ = researchDB.Kill();
        researchDB.CreateTables();

        _ = userDB.Kill();
        userDB.CreateTables();

    }
}
