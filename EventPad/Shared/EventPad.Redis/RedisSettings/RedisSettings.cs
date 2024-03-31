﻿namespace EventPad.Redis;

public class RedisSettings
{
    /// <summary>
    /// Time of the cache keeping (in minutes)
    /// </summary>
    public int CacheLifeTime { get; private set; }

    /// <summary>
    /// Redis connection string
    /// </summary>;
    public string Uri { get; private set; }
}
