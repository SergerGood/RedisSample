using Serilog;
using ServiceStack.Redis;

namespace Redis.Sample
{
    public static class ExamplesForIncrement
    {
        public static void Run(IRedisClientsManager clientsManager)
        {
            Log.Information("-- Increment --");

            var key = "key2";

            var redisNativeClient = clientsManager.GetClient() as IRedisNativeClient;
            using (var redis = redisNativeClient)
            {
                redis.Del(key);
                long value = redis.Incr(key);

                Log.Information("Increment key: {key}. New value: {value}", key, value);
            }

            using (var redis = redisNativeClient)
            {
                long value = redis.Decr(key);

                Log.Information("Decrement key: {key}. New value: {value}", key, value);
            }

            using (var redis = redisNativeClient)
            {
                var incrBy = 5;
                long value = redis.IncrBy(key, incrBy);

                Log.Information("Increment key: {key} with value: {incrBy}. New value: {value}", key, incrBy, value);
            }
        }
    }
}