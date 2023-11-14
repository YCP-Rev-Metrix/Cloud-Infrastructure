using System;
using System.Numerics;

namespace Common.POCOs;
public class Frame : POCO
{
    public Frame(int game_id, int score)
    {
        Game_id = game_id;
        Score = score;
    }
    public int Game_id { get; set; }
    public int Score { get; set;}

}

