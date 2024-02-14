﻿namespace Common.POCOs;

/// <summary>
/// Defines a POCO representing a single string-based token
/// </summary>
public class StringToken : POCO
{
    public StringToken(string token) => Token = token;

    public string Token { get; set; }
}
