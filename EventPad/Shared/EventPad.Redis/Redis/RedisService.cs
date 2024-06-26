﻿namespace EventPad.Redis;

using EventPad.Common;
using StackExchange.Redis;

public class RedisService : IRedisService
{
    private TimeSpan defaultLifetime = TimeSpan.FromSeconds(10);

    private readonly RedisSettings settings;
    private readonly IDatabase cacheDb;

    private static string redisUri;
    private static ConnectionMultiplexer Connection => lazyConnection.Value;
    private static Lazy<ConnectionMultiplexer> lazyConnection = new(() => ConnectionMultiplexer.Connect(redisUri));

    public RedisService(RedisSettings settings)
    {
        this.settings = settings;

        redisUri = this.settings.Uri;
        defaultLifetime = TimeSpan.FromSeconds(this.settings.CacheLifeTime);

        cacheDb = Connection.GetDatabase();
    }

    public string KeyGenerate()
    {
        return Guid.NewGuid().Shrink();
    }

    public async Task<bool> Delete(string key)
    {
        return await cacheDb.KeyDeleteAsync(key);
    }

    public async Task<T> Get<T>(string key, bool resetLifeTime = false)
    {
        try
        {
            string cached_data = await cacheDb.StringGetAsync(key);
            if (cached_data.IsNullOrEmpty())
                return default;

            var data = cached_data.FromJsonString<T>();

            if (resetLifeTime)
                await SetStoreTime(key);

            return data;
        }
        catch (Exception ex)
        {
            throw new Exception($"Can`t get data from cache for {key}", ex);
        }
    }

    public async Task<bool> Put<T>(string key, T data, TimeSpan? storeTime = null)
    {
        try
        {
            return await cacheDb.StringSetAsync(key, data.ToJsonString(), storeTime ?? defaultLifetime);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //public bool Put<T>(string key, T data, int storeTime = 60 * 60)
    //{
    //    return cacheDb.StringSet(key, System.Text.Json.JsonSerializer.Serialize(data), TimeSpan.FromSeconds(storeTime));
    //}

    public async Task SetStoreTime(string key, TimeSpan? storeTime = null)
    {
        await cacheDb.KeyExpireAsync(key, storeTime ?? defaultLifetime);
    }
}
