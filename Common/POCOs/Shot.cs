﻿using System.Numerics;

namespace Common.POCOs;
public class Shot : POCO
{
    public Shot()
    {

    }

    public Shot(int user_id,
                int? frame_id,
                int? ball_id,
                int? video_id,
                byte[] pins_remaining,
                DateTime time,
                byte[] lane_number,
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
        X_position = x_position;
        Y_position = y_position;
        Z_position = z_position;
        
    }

    public int User_id { get; set; }
    public int? Frame_id { get; set; }

    public int? Ball_id { get; set; }
    public int? Video_id { get; set; }
    public byte[] Pins_remaining { get; set; }
    public DateTime Time { get; set; }

    public byte[] Lane_Number { get; set; }
    public float Ddx { get; set; }
    public float Ddy { get; set; }
    public float Ddz { get; set; }
    public float X_position { get; set; }
    public float Y_position { get; set; }
    public float Z_position { get; set; }

}
