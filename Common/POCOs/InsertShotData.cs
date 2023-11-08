using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Common.POCOs;
public class InsertShotData : POCO
{
    // Will need to replacen sql binary with binary
    public InsertShotData(int User_id, int frame_id, int ball_id, int vid pins_remaining, DateTime time)
    {
        Frame_Id = frame_id;
        Ball_Id = ball_id;
        Video_Id = video_id;
        Pins_Remaining = pins_remaining;
        Time = time;
    }

    public int User_Id { get; set; }
    public int Frame_Id { get; set; }
    public int Ball_Id { get; set; }

    public int Video_Id { get; set; }

    public string Pins_Remaining { get; set; }

    public DateTime Time { get; set; }
}
