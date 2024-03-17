using Microsoft.SqlServer.Management.Smo;

namespace DatabaseCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var userDB = new UserDB();
        var researchDB = new ResearchDB();

        _ = userDB.Kill();
        userDB.CreateTables();


        _ = researchDB.Kill();
        researchDB.CreateTables();
        //researchDB.CreateUserDatabase();

        //userDB.database.Grant(Microsoft.SqlServer.Management.Smo.DatabasePermission.Select, researchDB.DatabaseName);


        

    }
}
