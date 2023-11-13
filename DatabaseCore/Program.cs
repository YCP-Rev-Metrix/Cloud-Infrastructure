namespace DatabaseCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var userDB = new UserDB();//ResearchDB ResearchDB = new ResearchDB();
        var researchDB = new ResearchDB();

        researchDB.Kill();
        researchDB.CreateTables();

        userDB.Kill();
        userDB.CreateTables();


    }
}
