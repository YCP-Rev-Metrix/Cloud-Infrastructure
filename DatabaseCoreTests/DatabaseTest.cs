using DatabaseCore;
using Microsoft.SqlServer.Management.Smo;
using TestCommon;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Common.POCOs;
using Xunit;
public class DatabaseTest : AbstractDatabase, IDisposable
{
    /// <summary>
    /// Create a instance of User database using Abstract Database to test the functions implemented
    /// Call Abstract -> using the base of revmetrix-u create a database.
    /// How will the environmental variable for Dockerized or Local host work when calling the test user database?
    /// Add tests for the functions
    /// !!DONE!!
    ///Create something like this
    /// Test build up (Unit testing), database testing (Unit testing), build down (Unit testing)
    /// </summary>


    public DatabaseTest(String DataBaseName = "revmetrix-u") : base(DataBaseName)
    {
        _ = new UserDB();
        UserDB use = new UserDB();
    }

    private static UserDB use = new UserDB();
    

    //private HashAndSalt hash = new HashAndSalt();

    //public bool DatabaseAddHash() => hash.Hash;

    [Fact]
    public bool DatabaseExist() => use.DoesExist().Equals(true);

    private string firstname = "Alex";
    private string lastname = "Boyer";
    private string password = "alexboyer";
    private static string username = "A-boy";
    private string roles = "user";
    private string phone = "717888888";
    private string email = "username@ycp.edu";

    
    private (bool success, byte[] salt, string roles, byte[] hashedPassword) result = use.GetUserValidData(username).Result;

    [Fact]
    public bool DatabaseCreateHashSalt() => result.success;


    [Fact]
    public Task<bool> DatabaseAddUser() => use.AddUser(firstname, lastname, username, result.hashedPassword, result.salt, roles, phone, email);

   


    [Fact]
    public void Test1() => Assert.True(true);

    public void Dispose()
    {
        // Cleanup resources if needed
       // _context.Dispose();
       
        // Optionally, you might want to delete the test database after the tests are complete
        if (DatabaseExist().Equals(true))
        {
            use?.Kill();    
        }
        //var dropDatabase = new Database(Server, "Testing");
        //dropDatabase.Drop();
        
    }

}


