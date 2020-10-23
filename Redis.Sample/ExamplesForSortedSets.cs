using Serilog;
using ServiceStack.Redis;

namespace Redis.Sample
{
    public static class ExamplesForSortedSets
    {
        public static void Run(IRedisClientsManager clientsManager)
        {
            Log.Information("-- Sorted Sets --");

            var key = "set3";

            var redisNativeClient = clientsManager.GetClient() as IRedisNativeClient;
            using (var redis = redisNativeClient)
            {
                redis.Del(key);

                for (int i = 0; i < 4; i++)
                {
                    var value = $"item {i}";
                    redis.ZAdd(key, i, value.GetBytes());

                    Log.Information("SetId: {key} Add value: {value} with score: {score}", key, value, i);
                }
            }

            using (var redis = redisNativeClient)
            {
                var min = 1;
                var max = 2;

                byte[][] values = redis.ZRangeByScore(key, min, max, null, null);

                foreach (var value in values)
                {
                    Log.Information("SetId: {key} From {from} to {to} value: {count}", key, min, max, value.GetString());
                }
            }
        }
    }
}