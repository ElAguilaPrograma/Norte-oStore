using Dapper;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public interface ICarritoRepository
    {
        Task<int> CreateAsync(Carrito carrito);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<Carrito>> GetAllAsync();
        Task<Carrito?> GetByIdAsync(int id);
        Task<int> UpdateAsync(Carrito carrito);
    }
    public class CarritoRepository: ICarritoRepository
    {
        private readonly DapperContext _context;

        public CarritoRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Carrito>> GetAllAsync()
        {
            var query = @"SELECT c.*, u.Nombre, a.Nombre_Articulo 
                          FROM carrito c
                          INNER JOIN usuarios u ON c.ID_Usuario = u.ID_Usuarios
                          INNER JOIN articulos a ON c.ID_Articulo = a.ID_Articulo";

            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Carrito, Usuario, Articulo, Carrito>(
                query,
                (carrito, usuario, articulo) =>
                {
                    carrito.Usuario = usuario;
                    carrito.Articulo = articulo;
                    return carrito;
                },
                splitOn: "Nombre,Nombre_Articulo"
            );
            return result;
        }

        public async Task<Carrito?> GetByIdAsync(int id)
        {
            var query = @"SELECT c.*, u.Nombre, a.Nombre_Articulo 
                          FROM carrito c
                          INNER JOIN usuarios u ON c.ID_Usuario = u.ID_Usuarios
                          INNER JOIN articulos a ON c.ID_Articulo = a.ID_Articulo
                          WHERE c.ID_Carrito_Item = @Id";

            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Carrito, Usuario, Articulo, Carrito>(
                query,
                (c, u, a) => { c.Usuario = u; c.Articulo = a; return c; },
                new { Id = id },
                splitOn: "Nombre,Nombre_Articulo"
            );
            return result.FirstOrDefault();
        }

        public async Task<int> CreateAsync(Carrito carrito)
        {
            var query = @"INSERT INTO carrito (ID_Usuario, ID_Articulo, Cantidad, Fecha_Agregado)
                          VALUES (@ID_Usuario, @ID_Articulo, @Cantidad, @Fecha_Agregado)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, carrito);
        }

        public async Task<int> UpdateAsync(Carrito carrito)
        {
            var query = @"UPDATE carrito 
                          SET ID_Usuario = @ID_Usuario, ID_Articulo = @ID_Articulo, 
                              Cantidad = @Cantidad, Fecha_Agregado = @Fecha_Agregado
                          WHERE ID_Carrito_Item = @ID_Carrito_Item";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, carrito);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query = "DELETE FROM carrito WHERE ID_Carrito_Item = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
