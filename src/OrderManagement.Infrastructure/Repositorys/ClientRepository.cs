using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OrderManagement.Domain.Entitys;
using Dapper;
using OrderManagement.Domain.Interfaces;


namespace OrderManagement.Infrastructure.Repositorys
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT Id, Name, Email, Phone, CreationDate FROM Clients";
                return await connection.QueryAsync<Client>(sql);
            }
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Clients WHERE Id = @Id";
                return await connection.QuerySingleOrDefaultAsync<Client>(sql, new { Id = id });
            }
        }

        public async Task AddAsync(Client client)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Clients (Name, Email, Phone, CreationDate) VALUES (@Name, @Email, @Phone, @CreationDate)";
                await connection.ExecuteAsync(sql, client);
            }
        }

        public async Task UpdateAsync(Client client)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Clients SET Name = @Name, Email = @Email, Phone = @Phone WHERE Id = @Id";
                await connection.ExecuteAsync(sql, client);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Clients WHERE Id = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
