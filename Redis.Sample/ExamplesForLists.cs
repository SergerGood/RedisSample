using Serilog;
using ServiceStack.Redis;

namespace Redis.Sample
{
    public static class ExamplesForLists
    {
        public static void Run(IRedisClientsManager clientsManager)
        {
            Log.Information("-- Lists --");

            var key = "key4";

            var redisNativeClient = clientsManager.GetClient() as IRedisNativeClient;
            using (var redis = redisNativeClient)
            {
                redis.Del(key);

                for (int i = 0; i < 3; i++)
                {
                    var value = $"item {i}";
                    redis.RPush(key, value.GetBytes());

                    Log.Information("ListId: {key} Push value: {value}", key, value);
                }
            }

            using (var redis = redisNativeClient)
            {
                var keepStartingFrom = 0;
                var keepEndingAt = 1;

                redis.LTrim(key, keepStartingFrom, keepEndingAt);

                Log.Information("ListId: {key} Keep from: {from} to: {to}", key, keepStartingFrom, keepEndingAt);
            }


            using (var redis = redisNativeClient)
            {
                byte[] valueL = redis.LPop(key);
                byte[] valueR = redis.RPop(key);

                Log.Information("ListId: {key} first: {valueL} last: {valueR}", key, valueL.GetString(),
                    valueR.GetString());
            }
        }
    }
}
