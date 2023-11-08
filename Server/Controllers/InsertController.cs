using Common.POCOs;
using DatabaseCore;
using Microsoft.AspNetCore.Mvc;
using Server;

[ApiController]
[Route("api/[controller]")]
public class InsertController : ControllerBase
{
    private readonly UserDB _userDB;
    private readonly ResearchDB _researchDB;

    public InsertController(UserDB userDB) => _userDB = userDB;

    [ProducesResponseType(typeof(DualToken), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost("Insert", Name = "Insert")]
    public async Task<IActionResult> Authorize([FromBody] UserIdentification userIdentification)
    {
        // Validate user credentials (e.g., check against a database)
        (bool success, string[]? roles) = await ServerState.UserStore.VerifyUser(userIdentification.Username, userIdentification.Password);
        if (success)
        {
            (string authorizationToken, byte[] refreshToken) = await ServerState.TokenStore.GenerateTokenSet(userIdentification.Username, roles);

            // Return the token as a response
            return Ok(new DualToken(authorizationToken, refreshToken));
        }

        // If credentials are invalid, return a 403 Forbid response
        return Forbid();
    }

    //public InsertController(ResearchDB researchDB) => _researchDB = researchDB;
    /*
    [HttpPost("CreateUser", Name = "CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] string name, [FromBody] string email, [FromBody] int phone_number)
    {
        if (name != null) 
        {
            try
            {
                await _userDB.Connection.OpenAsync();

                string insertQuery = "INSERT INTO [User] (name, email, phone) VALUES (@Name, @Email, @Phone)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, _userDB.Connection))
                {
                    insertCommand.Parameters.AddWithValue("@Name", name);
                    insertCommand.Parameters.AddWithValue("@Email", email);
                    insertCommand.Parameters.AddWithValue("@Phone", phone_number);
                    int rowsAffected = await insertCommand.ExecuteNonQueryAsync();
                }

                _userDB.Connection.Close();

                return Ok("User created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        return BadRequest("Invalid input");
    }*/

    // Other actions for updating and deleting items
}
