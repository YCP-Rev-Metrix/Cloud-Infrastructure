using System;

namespace Common.POCOs;

public class Shot : POCO
{
    // Parameterless constructor needed for model binding
    public Shot() { }

    // Parameterized constructor can still exist and be used elsewhere
    public Shot(long user_id, long? frame_id, long? ball_id, long? video_id, long pins_remaining,
                DateTime time, long lane_number, double ddx, double ddy, double ddz,
                double x_position, double y_position, double z_position, long? pocket_hit)
    {
        User_id = user_id;
        Frame_id = frame_id;
        Ball_id = ball_id;
        Video_id = video_id;
        Pins_remaining = pins_remaining;
        Time = time;
        Lane_Number = lane_number;
        Ddx = ddx;
        Ddy = ddy;
        Ddz = ddz;
        X_position = x_position;
        Y_position = y_position;
        Z_position = z_position;
        Pocket_hit = pocket_hit;
    }

    // Properties
    public long User_id { get; set; }
    public long? Frame_id { get; set; }
    public long? Ball_id { get; set; }
    public long? Video_id { get; set; }
    public long Pins_remaining { get; set; }
    public DateTime Time { get; set; }
    public long Lane_Number { get; set; }
    public double Ddx { get; set; }
    public double Ddy { get; set; }
    public double Ddz { get; set; }
    public double X_position { get; set; }
    public double Y_position { get; set; }
    public double Z_position { get; set; }
    public long? Pocket_hit { get; set; }
}
