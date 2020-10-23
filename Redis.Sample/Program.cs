using System;
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

            ExamplesForString.Run(clientsManager);
            ExamplesForIncrement.Run(clientsManager);
            ExamplesForHashes.Run(clientsManager);
            ExamplesForLists.Run(clientsManager);
            ExamplesForSets.Run(clientsManager);
            ExamplesForSortedSets.Run(clientsManager);

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
    }
}