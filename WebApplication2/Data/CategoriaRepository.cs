using Dapper;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public interface ICategoriaRepository
    {
        Task<int> CreateAsync(Categoria categoria);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task<int> UpdateAsync(Categoria categoria);
    }
    public class CategoriaRepository: ICategoriaRepository
    {
        private readonly DapperContext _context;

        public CategoriaRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            var query = "SELECT * FROM categorias";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Categoria>(query);
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM categorias WHERE ID_Categorias = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Categoria>(query, new { Id = id });
        }

        public async Task<int> CreateAsync(Categoria categoria)
        {
            var query = "INSERT INTO categorias (Nombre_Categoria) VALUES (@Nombre_Categoria)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, categoria);
        }

        public async Task<int> UpdateAsync(Categoria categoria)
        {
            var query = "UPDATE categorias SET Nombre_Categoria = @Nombre_Categoria WHERE ID_Categorias = @ID_Categorias";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, categoria);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query = "DELETE FROM categorias WHERE ID_Categorias = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
