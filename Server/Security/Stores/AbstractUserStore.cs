using System.Numerics;

namespace Server.Security.Stores;

public abstract class AbstractUserStore
{
    public abstract Task<bool> CreateUser(string firstname, string lastname, string username, string password, string email, string phone_number, string[]? roles = null);
    public abstract Task<bool> DeleteUser(string username);
    public abstract Task<bool> InsertShot(long user_id,
                                       long? frame_id,
                                       long? ball_id,
                                       long? video_id,
                                       long pins_remaining,
                                       DateTime time,
                                       long lane_number,
                                       double ddx,
                                       double ddy,
                                       double ddz,
                                       double x_position,
                                       double y_position,
                                       double z_position, 
                                       long? pocket_hit);
    public abstract Task<bool> InsertBall(float weight, string? color);
    public abstract Task<(bool success, string[]? roles)> GetRoles(string username);
    public abstract Task<(bool success, string[]? roles)> VerifyUser(string username, string password);

    public abstract Task<bool> StartSession(//int user_id,
                                                    int? leauge_id,
                                                    int? tournament_id,
                                                    int? practice_id,
                                                    DateTime time,
                                                    string location,
                                                    int total_games,
                                                    int total_frames);
    public abstract Task<bool> StartPractice(int event_id);

    public abstract Task<bool> StartEvent(int user_id, string event_type);

    public abstract Task<bool> StartGame(int session_id, int score);

    public abstract Task<bool> StartFrame(int game_id, int shot_number, int score);


}
