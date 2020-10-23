using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace Redis.Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IRedisClientsManager>(new RedisManagerPool("localhost:6379"))
                .BuildServiceProvider();

            var clientsManager = serviceProvider
                .GetService<IRedisClientsManager>();

            RunExamplesForString(clientsManager);
            RunExamplesForIncrement(clientsManager);
            RunExamplesForHashes(clientsManager);
            RunExamplesForLists(clientsManager);
            RunExamplesForSets(clientsManager);
            RunExamplesForSortedSets(clientsManager);

            using (var redis = clientsManager.GetClient())
            {
                var redisUsers = redis.As<User>();

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "a@b.r",
                    City = "Oslo",
                    Name = "Bob"
                };

                redisUsers.SetValue(user.Id, user);


                var redisUsersLookup = redis.As<string>();

                IRedisHash<string, string> hash = redisUsersLookup.GetHash<string>("user:lookup");

                redisUsersLookup.SetEntryInHash(hash, user.Email, user.Id);

                string userId = redisUsersLookup.GetValueFromHash(hash, user.Email);


                var storedUser = redisUsers.GetValue(userId);

            }
        }

        private static void RunExamplesForSortedSets(IRedisClientsManager clientsManager)
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
                    Log.Information("SetId: {key} From {from} to {to} value: {count}", key, min, max,
                        value.GetString());
            }
        }

        private static void RunExamplesForSets(IRedisClientsManager clientsManager)
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
                    Log.Information("SetId: {key1} and SetId: {key2} Intersect value: {value}", key1, key2,
                        value.GetString());
            }
        }

        private static void RunExamplesForLists(IRedisClientsManager clientsManager)
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

        private static void RunExamplesForHashes(IRedisClientsManager clientsManager)
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

        private static void RunExamplesForIncrement(IRedisClientsManager clientsManager)
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

        private static void RunExamplesForString(IRedisClientsManager clientsManager)
        {
            Log.Information("-- Strings --");

            var key = "key1";
            var value = new byte[] {1, 2, 3};

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
                var bytes = new byte[] {4};
                long length = redis.Append(key, bytes);
                byte[] blob = redis.Get(key);

                Log.Information("Append bytes: {bytes}. New length {length}. New value {blob}",
                    BitConverter.ToString(bytes), length, BitConverter.ToString(blob));
            }
        }
    }

    internal class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
    }
}