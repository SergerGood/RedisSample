using System.Linq;
using Serilog;
using ServiceStack.Redis;

namespace Redis.Sample
{
    public static class ExamplesForHashes
    {
        public static void Run(IRedisClientsManager clientsManager)
        {
            Log.Information("-- Hashes --");

            var key = "key3";

            var redisNativeClient = clientsManager.GetClient() as IRedisNativeClient;
            using (var redis = redisNativeClient)
            {
                string fieldName = "name";
                string fieldValue = "Bob";

                redis.Del(key);
                redis.HSet(key, fieldName.GetBytes(), fieldValue.GetBytes());

                Log.Information("Set value: {value} field name: {name} to hashId: {key}", fieldValue, fieldName, key);
            }

            using (var redis = redisNativeClient)
            {
                string fieldName = "name";
                byte[] value = redis.HGet(key, fieldName.GetBytes());

                Log.Information("Get value: {value} for field name: {name} from hashId: {key} ", value.GetString(),
                    fieldName,
                    key);
            }

            using (var redis = redisNativeClient)
            {
                var properties = new byte[2][];

                var property1 = "surname";
                properties[0] = property1.GetBytes();

                var property2 = "age";
                properties[1] = property2.GetBytes();

                var values = new byte[2][];
                values[0] = "Zhlob".GetBytes();
                values[1] = "5".GetBytes();

                redis.HMSet(key, properties, values);

                Log.Information("Set hashId: {key} properties: {property1}, {property2}", key, property1, property2);
            }

            using (var redis = redisNativeClient)
            {
                var fieldName = "surname";
                redis.HDel(key, fieldName.GetBytes());

                Log.Information("Del property {property}  for hashId: {key}", fieldName, key);
            }

            using (var redis = redisNativeClient)
            {
                byte[][] values = redis.HGetAll(key);

                Log.Information("Get all {values}", string.Join(", ", values.Select(x => x.GetString())));
            }
        }
    }
}