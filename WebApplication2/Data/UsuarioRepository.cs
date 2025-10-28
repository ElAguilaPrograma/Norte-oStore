using Dapper;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public interface IUsuarioRepository
    {
        Task<int> CreateAsync(Usuario usuario);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<int> UpdateAsync(Usuario usuario);
    }
    public class UsuarioRepository: IUsuarioRepository
    {
        private readonly DapperContext _context;

        public UsuarioRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var query = @"SELECT u.*, r.Nombre_Rol 
                          FROM usuarios u 
                          INNER JOIN roles r ON u.ID_Rol = r.ID_Rol";

            using var connection = _context.CreateConnection();
            var usuarios = await connection.QueryAsync<Usuario, Rol, Usuario>(
                query,
                (usuario, rol) =>
                {
                    usuario.Rol = rol;
                    return usuario;
                },
                splitOn: "Nombre_Rol"
            );
            return usuarios;
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            var query = @"SELECT u.*, r.Nombre_Rol 
                          FROM usuarios u 
                          INNER JOIN roles r ON u.ID_Rol = r.ID_Rol
                          WHERE u.ID_Usuarios = @Id";

            using var connection = _context.CreateConnection();
            var usuario = await connection.QueryAsync<Usuario, Rol, Usuario>(
                query,
                (u, r) => { u.Rol = r; return u; },
                new { Id = id },
                splitOn: "Nombre_Rol"
            );
            return usuario.FirstOrDefault();
        }

        public async Task<int> CreateAsync(Usuario usuario)
        {
            var query = @"INSERT INTO usuarios (Nombre, Email, Contraseña, ID_Rol) 
                          VALUES (@Nombre, @Email, @Contraseña, @ID_Rol)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, usuario);
        }

        public async Task<int> UpdateAsync(Usuario usuario)
        {
            var query = @"UPDATE usuarios 
                          SET Nombre = @Nombre, Email = @Email, Contraseña = @Contraseña, ID_Rol = @ID_Rol 
                          WHERE ID_Usuarios = @ID_Usuarios";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, usuario);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query = "DELETE FROM usuarios WHERE ID_Usuarios = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
