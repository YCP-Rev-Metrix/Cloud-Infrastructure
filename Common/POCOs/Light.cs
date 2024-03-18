using System;
using System.Numerics;

namespace Common.POCOs;
public class Light : POCO
{
    public Light(float[] light_levels)
    {
        Light_levels = light_levels;
    }

    public float[] Light_levels{ get; set; }


}
