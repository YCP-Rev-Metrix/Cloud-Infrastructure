using System.Numerics;

namespace Common.POCOs;
public class Shot : POCO
{
    public Shot(int user_id,
                int frame_id,
                int ball_id,
                int video_id,
                BinaryData pins_remaining,
                DateTime time,
                BinaryData lane_number,
                float ddx,
                float ddy,
                float ddz,
                float x_position,
                float y_position,
                float z_position)
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
        Dx = x_position;
        Dy = y_position;
        Dz = z_position;

    }

    public BigInteger User_id { get; set; }
    public BigInteger Frame_id { get; set; }

    public BigInteger Ball_id { get; set; }
    public BigInteger Video_id { get; set; }
    public BinaryData Pins_remaining { get; set; }
    public DateTime Time { get; set; }

    public BinaryData Lane_Number { get; set; }
    public float Ddx { get; set; }
    public float Ddy { get; set; }
    public float Ddz { get; set; }
    public float Dx { get; set; }
    public float Dy { get; set; }
    public float Dz { get; set; }

}
