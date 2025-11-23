using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using eShop.Services.Basket.API.Model;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace eShop.Services.Basket.API.Infrastructure.Repositories {
    public class RedisBasketRepository : IBasketRepository {
        private readonly ILogger<RedisBasketRepository> logger;
        private readonly ConnectionMultiplexer redis;
        private readonly IDatabase database;

        public RedisBasketRepository(ILogger<RedisBasketRepository> logger,
            ConnectionMultiplexer redis) {
            this.logger = logger;
            this.redis = redis;
            this.database = redis.GetDatabase();
        }

        public async Task<CustomerBasket> GetBasketAsync(string buyerID) {
            RedisValue value = await this.database.StringGetAsync(buyerID);

            if (value.IsNullOrEmpty) {
                return null;
            }

            return JsonSerializer.Deserialize<CustomerBasket>(value, new JsonSerializerOptions() {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<CustomerBasket> AddOrUpdateBasketAsync(CustomerBasket basket) {
            bool wasCreated = await this.database.StringSetAsync(
                basket.BuyerID,
                JsonSerializer.Serialize(basket)
            );

            if (!wasCreated) {
                this.logger.LogInformation(
                    $"Problem occurred persisting {nameof(CustomerBasket)}" +
                    $" with BuyerID = {basket.BuyerID}"
                );
                return null;
            }

            this.logger.LogInformation(
                $"Basket with BuyerID = {basket.BuyerID} was persisted successfully"
            );

            return await GetBasketAsync(basket.BuyerID);
        }

        public async Task<bool> DeleteBasketAsync(string id) {
            return await this.database.KeyDeleteAsync(id);
        }

        public IEnumerable<string> GetUsers() {
            IServer server = this.GetServer();
            IEnumerable<RedisKey> data = server.Keys();

            return data?.Select(x => x.ToString());
        }

        private IServer GetServer() {
            EndPoint[] endpoint = this.redis.GetEndPoints();
            return this.redis.GetServer(endpoint.First());
        }
    }
}