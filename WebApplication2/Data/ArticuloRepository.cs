using Dapper;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public interface IArticuloRepository
    {
        Task<int> CreateAsync(Articulo articulo);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<Articulo>> GetAllAsync();
        Task<Articulo?> GetByIdAsync(int id);
        Task<int> UpdateAsync(Articulo articulo);
    }

    public class ArticuloRepository : IArticuloRepository
    {
        private readonly DapperContext _context;

        public ArticuloRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Articulo>> GetAllAsync()
        {
            // MODIFICADO: Incluir ImagenUrl en el SELECT
            var query = @"SELECT a.*, c.Nombre_Categoria 
                          FROM articulos a 
                          INNER JOIN categorias c ON a.ID_Categoria = c.ID_Categorias";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Articulo, Categoria, Articulo>(
                query,
                (articulo, categoria) => {
                    articulo.Categoria = categoria;
                    return articulo;
                },
                splitOn: "Nombre_Categoria"
            );
        }

        public async Task<Articulo?> GetByIdAsync(int id)
        {
            // MODIFICADO: Incluir ImagenUrl en el SELECT
            var query = @"SELECT a.*, c.Nombre_Categoria 
                          FROM articulos a 
                          INNER JOIN categorias c ON a.ID_Categoria = c.ID_Categorias
                          WHERE a.ID_Articulo = @Id";

            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Articulo, Categoria, Articulo>(
                query,
                (a, c) => {
                    a.Categoria = c;
                    return a;
                },
                new { Id = id },
                splitOn: "Nombre_Categoria"
            );
            return result.FirstOrDefault();
        }

        public async Task<int> CreateAsync(Articulo articulo)
        {
            // MODIFICADO: Incluir ImagenUrl en el INSERT
            var query = @"INSERT INTO articulos (Nombre_Articulo, Descripcion, Precio, Stock, ID_Categoria, ImagenUrl)
                          VALUES (@Nombre_Articulo, @Descripcion, @Precio, @Stock, @ID_Categoria, @ImagenUrl)";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, articulo);
        }

        public async Task<int> UpdateAsync(Articulo articulo)
        {
            // MODIFICADO: Incluir ImagenUrl en el UPDATE
            var query = @"UPDATE articulos 
                          SET Nombre_Articulo = @Nombre_Articulo, 
                              Descripcion = @Descripcion, 
                              Precio = @Precio, 
                              Stock = @Stock, 
                              ID_Categoria = @ID_Categoria,
                              ImagenUrl = @ImagenUrl
                          WHERE ID_Articulo = @ID_Articulo";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, articulo);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query = "DELETE FROM articulos WHERE ID_Articulo = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}