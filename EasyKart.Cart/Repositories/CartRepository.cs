using EasyKart.Shared.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace EasyKart.Cart.Repositories
{
    public class CartRepository : ICartRepository
    {
        IConfiguration _configuration;
        private readonly string _cosmosEndpoint;
        private readonly string _cosmosKey;
        private readonly string _databaseId;
        private readonly string _containerId;
        private readonly string _partitionKey;

        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public CartRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosEndpoint = _configuration["CosmosDB:endpoint"];
            _cosmosKey = _configuration["CosmosDB:authKey"];
            _databaseId = _configuration["CosmosDB:databaseId"];
            _containerId = _configuration["CosmosDB:containerId"];
            _partitionKey = _configuration["CosmosDB:partitionKey"];

            _cosmosClient = new CosmosClient(_cosmosEndpoint, _cosmosKey);
            _container = _cosmosClient.GetContainer(_databaseId, _containerId);
        }

        public async Task<bool> AddItemToCartAsync(EasyKart.Shared.Models.Cart cart)
        {
            try
            {
                await _container.CreateItemAsync(cart, new PartitionKey(cart.UserId.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }           
        }

        // Get cart by user id
        public async Task<EasyKart.Shared.Models.Cart> GetCartAsync(Guid userId)
        {
            try
            {
                var sqlQueryText = $"SELECT * FROM c WHERE c.userId = '{userId}'";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<EasyKart.Shared.Models.Cart> queryResultSetIterator = _container.GetItemQueryIterator<EasyKart.Shared.Models.Cart>(queryDefinition);
                List<EasyKart.Shared.Models.Cart> carts = new List<EasyKart.Shared.Models.Cart>();
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<EasyKart.Shared.Models.Cart> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (EasyKart.Shared.Models.Cart cart in currentResultSet)
                    {
                        carts.Add(cart);
                    }
                }
                return carts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Update cart
        public async Task<bool> UpdateCartAsync(EasyKart.Shared.Models.Cart cart)
        {
            try
            {
                await _container.UpsertItemAsync(cart, new PartitionKey(cart.UserId.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Empty cart
        public async Task<bool> EmptyCartAsync(Guid userId)
        {
            try
            {
                var cart = await GetCartAsync(userId);
                if (cart != null)
                {
                    cart.Items.Clear();
                    await _container.UpsertItemAsync(cart, new PartitionKey(cart.UserId.ToString()));
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
