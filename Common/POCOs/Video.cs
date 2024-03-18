using System;
using System.Numerics;

namespace Common.POCOs;
public class Video : POCO
{
    public Video(float video, int video_id)
    {
        Video_file = video;
        Video_id = video_id;
    }

    public float Video_file { get; set; }
    public int Video_id { get; set; }

}
