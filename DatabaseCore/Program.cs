namespace DatabaseCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var userDB = new UserDB();//ResearchDB ResearchDB = new ResearchDB();
        var researchDB = new ResearchDB();

        researchDB.Kill();
        researchDB.CreateTables();

        if(userDB.DoesExist())
        {
            userDB.Kill();
            userDB.CreateTables();
        }
        else
        {
            Console.WriteLine("The Database doesn't exist");
            userDB.CreateTables();
        }

        
    }
}
