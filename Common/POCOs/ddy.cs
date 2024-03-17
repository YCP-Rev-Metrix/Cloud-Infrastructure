using System;
using System.Numerics;

namespace Common.POCOs;
public class ddy : POCO
{
    public ddy(float accelerations, int ball_id)
    {
        Accelerations = accelerations;
        Ball_id = ball_id;
    }

    public float Accelerations { get; set; }
    public int Ball_id { get; set; }

}
