using System;
using Serilog;
using ServiceStack.Redis;

namespace Redis.Sample
{
    public static class ExamplesForString
    {
        public static void Run(IRedisClientsManager clientsManager)
        {
            Log.Information("-- Strings --");

            var key = "key1";
            var value = new byte[] { 1, 2, 3 };

            var redisNativeClient = clientsManager.GetClient() as IRedisNativeClient;
            using (var redis = redisNativeClient)
            {
                redis.Del(key);
                redis.SetEx(key, 10, value);
                Log.Information("key: {key} value: {value}", key, BitConverter.ToString(value));

                byte[] blob = redis.Get(key);

                Log.Information("Get value by key: {value}", BitConverter.ToString(blob));
            }

            using (var redis = redisNativeClient)
            {
                long length = redis.StrLen(key);

                Log.Information("Get value length: {length}", length);
            }

            using (var redis = redisNativeClient)
            {
                var fromIndex = 1;
                var toIndex = 2;

                byte[] range = redis.GetRange(key, fromIndex, toIndex);

                Log.Information("Get value range from {fromIndex} to {toIndex}: {range}", fromIndex, toIndex,
                    BitConverter.ToString(range));
            }

            using (var redis = redisNativeClient)
            {
                var bytes = new byte[] { 4 };
                long length = redis.Append(key, bytes);
                byte[] blob = redis.Get(key);

                Log.Information("Append bytes: {bytes}. New length {length}. New value {blob}",
                    BitConverter.ToString(bytes), length, BitConverter.ToString(blob));
            }
        }
    }
}