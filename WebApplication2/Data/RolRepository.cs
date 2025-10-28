using Dapper;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public interface IRolRepository
    {
        Task<int> CreateAsync(Rol rol);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(int id);
        Task<int> UpdateAsync(Rol rol);
    }
    public class RolRepository: IRolRepository
    {
        private readonly DapperContext _context;

        public RolRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            var query = "SELECT * FROM roles";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Rol>(query);
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM roles WHERE ID_Rol = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Rol>(query, new { Id = id });
        }

        public async Task<int> CreateAsync(Rol rol)
        {
            var query = "INSERT INTO roles (Nombre_Rol) VALUES (@Nombre_Rol)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, rol);
        }

        public async Task<int> UpdateAsync(Rol rol)
        {
            var query = "UPDATE roles SET Nombre_Rol = @Nombre_Rol WHERE ID_Rol = @ID_Rol";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, rol);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query = "DELETE FROM roles WHERE ID_Rol = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
