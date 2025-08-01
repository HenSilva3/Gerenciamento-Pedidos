using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OrderManagement.Domain.Entitys;
using OrderManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Infrastructure.Repositorys
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
                
        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT Id, Name, Description, Price, StockQuantity FROM Products";
                return await connection.QueryAsync<Product>(sql);
            }
        }
        
        public async Task<Product> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Products WHERE Id = @Id";
                return await connection.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
            }
        }
                
        public async Task AddAsync(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Products (Name, Description, Price, StockQuantity) VALUES (@Name, @Description, @Price, @StockQuantity)";
                await connection.ExecuteAsync(sql, product);
            }
        }
        
        public async Task UpdateAsync(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, StockQuantity = @StockQuantity WHERE Id = @Id";
                await connection.ExecuteAsync(sql, product);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Products WHERE Id = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });
            }
        }
        
        public async Task UpdateStockAsync(int productId, int newQuantity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Products SET StockQuantity = @NewQuantity WHERE Id = @ProductId";
                await connection.ExecuteAsync(sql, new { NewQuantity = newQuantity, ProductId = productId });
            }
        }
    }
}
