using System;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

internal interface IRedisClient : IDisposable
{
    string? Get(string key);

    bool Set(string key, string value, TimeSpan? expiry);

    bool Delete(string key);
}
