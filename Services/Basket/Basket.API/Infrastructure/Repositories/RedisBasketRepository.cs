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
                this.logger.LogInformation(
                    "Basket with BuyerID = {BuyerID} was not found",
                    buyerID
                );
                return null;
            }

            CustomerBasket customerBasket = JsonSerializer.Deserialize<CustomerBasket>(
                value,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
            );

            this.logger.LogInformation(
                "Basket with BuyerID = {BuyerID} was found",
                customerBasket.BuyerID
            );

            return customerBasket;
        }

        public async Task<CustomerBasket> AddOrUpdateBasketAsync(CustomerBasket basket) {
            bool wasCreated = await this.database.StringSetAsync(
                basket.BuyerID,
                JsonSerializer.Serialize(basket)
            );

            if (!wasCreated) {
                this.logger.LogError(
                    "Problem occurred persisting {EntityName} with BuyerID = {BuyerID}",
                    nameof(CustomerBasket),
                    basket.BuyerID
                );
                return null;
            }

            this.logger.LogInformation(
                "Basket with BuyerID = {BuyerID} was persisted successfully",
                basket.BuyerID
            );

            return await GetBasketAsync(basket.BuyerID);
        }

        public async Task<bool> DeleteBasketAsync(string id) {
            bool result = await this.database.KeyDeleteAsync(id);
            if (result) {
                this.logger.LogInformation("Basket with BuyerID = {BuyerID} was deleted", id);
            } else {
                this.logger.LogError("Problem occurred deleting Basket with BuyerID = {BuyerID}", id);
            }

            return result;
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