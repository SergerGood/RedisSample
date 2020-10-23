using Serilog;
using ServiceStack.Redis;

namespace Redis.Sample
{
    public static class ExamplesForSets
    {
        public static void Run(IRedisClientsManager clientsManager)
        {
            Log.Information("-- Sets --");

            var key1 = "set1";
            var key2 = "set2";

            var redisNativeClient = clientsManager.GetClient() as IRedisNativeClient;
            using (var redis = redisNativeClient)
            {
                redis.Del(key1);
                redis.Del(key2);

                for (int i = 0; i < 2; i++)
                {
                    var value = $"item {i}";
                    redis.SAdd(key1, value.GetBytes());

                    Log.Information("SetId: {key} Add value: {value}", key1, value);
                }

                for (int i = 1; i < 3; i++)
                {
                    var value = $"item {i}";
                    redis.SAdd(key2, value.GetBytes());

                    Log.Information("SetId: {key} Add value: {value}", key2, value);
                }
            }

            using (var redis = redisNativeClient)
            {
                byte[][] values = redis.SInter(key1, key2);

                foreach (var value in values)
                {
                    Log.Information("SetId: {key1} and SetId: {key2} Intersect value: {value}", key1, key2, value.GetString());
                }
            }
        }
    }
}