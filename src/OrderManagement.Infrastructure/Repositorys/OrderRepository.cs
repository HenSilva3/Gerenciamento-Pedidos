using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OrderManagement.Domain.Entitys;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Infrastructure.Repositorys
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddAsync(Order order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var orderSql = @"INSERT INTO Orders (ClientId, OrderDate, TotalPrice, Status) 
                                         VALUES (@ClientId, @OrderDate, @TotalPrice, @Status);
                                         SELECT CAST(SCOPE_IDENTITY() as int)";

                        var orderId = await connection.QuerySingleAsync<int>(orderSql, order, transaction);

                        foreach (var item in order.Items)
                        {
                            item.OrderId = orderId;
                        }

                        var itemsSql = @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) 
                                         VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";

                        await connection.ExecuteAsync(itemsSql, order.Items, transaction);

                        transaction.Commit();
                        return orderId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Orders ORDER BY OrderDate DESC";
                return await connection.QueryAsync<Order>(sql);
            }
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT * FROM Orders WHERE Id = @Id;
                            SELECT * FROM OrderItems WHERE OrderId = @Id;";

                using (var multi = await connection.QueryMultipleAsync(sql, new { Id = id }))
                {
                    var order = await multi.ReadSingleOrDefaultAsync<Order>();

                    if (order != null)
                    {
                        order.Items = (await multi.ReadAsync<OrderItem>()).ToList();
                    }

                    return order;
                }
            }
        }

        public async Task UpdateStatusAsync(int orderId, string newStatus)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Orders SET Status = @NewStatus WHERE Id = @OrderId";
                await connection.ExecuteAsync(sql, new { NewStatus = newStatus, OrderId = orderId });
            }
        }
    }
}
