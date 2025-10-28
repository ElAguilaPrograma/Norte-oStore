using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly DapperContext _context;

        public UsuarioController(IUsuarioRepository usuarioRepository, DapperContext context)
        {
            _usuarioRepository = usuarioRepository;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioRegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest("Datos invalidos");
            }

            // Verificar si el usuario ya exite en la base de datos
            var usuarios = await _usuarioRepository.GetAllAsync();
            var existe = usuarios.Any(u => u.Email == request.Email);
            if (existe)
            {
                return BadRequest("El email ya esta registrado");
            }

            request.ID_Rol = 2;

            // Crear un nuevo usuario
            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Email = request.Email,
                Contraseña = request.Password,
                ID_Rol = request.ID_Rol
            };

            var filasAfectadas = await _usuarioRepository.CreateAsync(usuario);

            if (filasAfectadas == 0)
            {
                return StatusCode(500, "No se pudo registrar al usuario");
            }

            return Ok("Usuario registrado correctamente");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginRequest request)
        {
            if (request == null)
            {
                return BadRequest("Datos inválidos");
            }

            var usuarios = await _usuarioRepository.GetAllAsync();
            var usuario = usuarios.FirstOrDefault(u => u.Email == request.Email && u.Contraseña == request.Password);

            if (usuario == null)
            {
                return NotFound("Correo o contraseña incorrectos");
            }

            string rol = usuario.ID_Rol == 1 ? "Admin" : "Cliente";

            var response = new
            {
                mensaje = "Inicio de sesión exitoso",
                nombre = usuario.Nombre,
                email = usuario.Email,
                ID_Rol = rol
            };

            Console.WriteLine($"Login exitoso: {usuario.Email}, Rol: {rol}, ID_Rol: {usuario.ID_Rol}");

            return Ok(response);
        }

    }
}
