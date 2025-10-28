using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly IArticuloRepository _articuloRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductosController(DapperContext context, 
            IArticuloRepository articuloRepository, 
            ICategoriaRepository categoriaRepository,
            IWebHostEnvironment environment)
        {
            _context = context;
            _articuloRepository = articuloRepository;
            _categoriaRepository = categoriaRepository;
            _environment = environment;
        }

        [HttpGet("mostrarproductos")]
        public async Task<IActionResult> MostrarProductos()
        {
            var productos = await _articuloRepository.GetAllAsync();
            
            return Ok(productos);
        }

        [HttpPost("crearnuevoproducto")]
        public async Task<IActionResult> CrearProductos([FromForm] ProductoRequest request)
        {
            if (request == null)
            {
                return BadRequest("Datos invalidos");
            }

            // Verificar si el articulo ya existe en la base de datos
            var productos = await _articuloRepository.GetAllAsync();
            var existe = productos.Any(p => p.Nombre_Articulo == request.Nombre);
            if (existe)
            {
                return BadRequest("El producto ya esta registrado");
            }

            // Validar que se haya subido una imagen
            if (request.Imagen == null || request.Imagen.Length == 0)
            {
                return BadRequest("Debe subir una imagen para el producto");
            }

            var nombreCategoria = request.NombreCategoria;
            var categoria = await _categoriaRepository.GetAllAsync();

            var categoriaDeProducto = categoria.FirstOrDefault(c => c.Nombre_Categoria == nombreCategoria);
            if (categoriaDeProducto == null)
            {
                return NotFound("No se encontro la categoria");
            }
            var categoriaId = categoriaDeProducto.ID_Categorias;

            string imagenUrl = await GuardarImagen(request.Imagen);

            var produto = new Articulo
            {
                Nombre_Articulo = request.Nombre,
                Precio = request.Precio,
                Stock = request.Stock,
                ID_Categoria = categoriaId,
                ImagenUrl = imagenUrl
            };

            var filasAfectadas = await _articuloRepository.CreateAsync(produto);
            if (filasAfectadas == 0)
            {
                // Si falla la creación, eliminar la imagen guardada
                if (!string.IsNullOrEmpty(imagenUrl))
                {
                    EliminarImagen(imagenUrl);
                }
                return StatusCode(500, "No se pudo crear el producto");
            }

            return Ok("Producto Creado con Exito");
        }

        [HttpPut("editarproducto/{productoId}")]
        public async Task<IActionResult> EditarProducto([FromForm] ProductoRequest request, int productoId)
        {
            if (request == null)
            {
                return BadRequest("Datos invalidos");
            }

            // Vericar que exista el producto antes de editarlo
            var productos = await _articuloRepository.GetAllAsync();
            var producto = productos.FirstOrDefault(p => p.ID_Articulo == productoId);
            if (producto == null)
            {
                return NotFound("No se encontro el producto");
            }

            var categorias = await _categoriaRepository.GetAllAsync();
            var categoria = categorias.FirstOrDefault(c => c.Nombre_Categoria == request.NombreCategoria);
            if (categoria == null)
            {
                return NotFound("No se encontró la categoría");
            }

            string nuevaImagenUrl = producto.ImagenUrl; // Mantener la imagen actual por defecto

            if (request.Imagen != null && request.Imagen.Length > 0)
            {
                // Guardar nueva imagen
                nuevaImagenUrl = await GuardarImagen(request.Imagen);

                // Eliminar imagen anterior si existe
                if (!string.IsNullOrEmpty(producto.ImagenUrl))
                {
                    EliminarImagen(producto.ImagenUrl);
                }
            }

            // Actualizar los productos
            producto.Nombre_Articulo = request.Nombre;
            producto.Precio = request.Precio;
            producto.Stock = request.Stock;
            producto.ID_Categoria = categoria.ID_Categorias;
            producto.ImagenUrl = nuevaImagenUrl;

            // Guardar cambios
            var filasAfectadas = await _articuloRepository.UpdateAsync(producto); 
            if (filasAfectadas == 0)
            {
                return StatusCode(500, "No se pudo actualizar el producto");
            }

            return Ok("Producto actualizado con éxito");
        }

        [HttpDelete("borrarproducto/{productoId}")]
        public async Task<IActionResult> BorrarProducto (int productoId)
        {
            var productos = await _articuloRepository.GetAllAsync();
            var existe = productos.FirstOrDefault(p => p.ID_Articulo == productoId);
            if (existe == null)
            {
                return NotFound("No se encontro el producto a eliminar");
            }

            if (!string.IsNullOrEmpty(existe.ImagenUrl))
            {
                EliminarImagen(existe.ImagenUrl);
            }

            var filasAfectadas = await _articuloRepository.DeleteAsync(productoId);

            if (filasAfectadas == 0)
            {
                return StatusCode(500, "No se pudo borrar el producto");
            }

            return Ok("Producto borrado con éxito");
        }

        [HttpGet("mostrarporcategoria/{nombreCategoria}")]
        public async Task<IActionResult> MostrarPorCategoria(string nombreCategoria)
        {
            var productos = await _articuloRepository.GetAllAsync();
            var filtrados = productos
                .Where(p => p.Categoria != null && p.Categoria.Nombre_Categoria.Equals(nombreCategoria, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!filtrados.Any())
            {
                return NotFound($"No se encontroran podructos de esta categoria {nombreCategoria}");
            }

            return Ok(filtrados);
        }

        private async Task<string> GuardarImagen(IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0)
                return null;

            // Crear carpeta si no existe
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "Imagenes", "Productos");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generar nombre único para el archivo
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            // Retornar URL relativa (así la puede usar el frontend)
            return $"/Imagenes/Productos/{fileName}";
        }
        private void EliminarImagen(string imagenUrl)
        {
            if (string.IsNullOrEmpty(imagenUrl))
                return;

            try
            {
                var fileName = Path.GetFileName(imagenUrl);
                var filePath = Path.Combine(_environment.WebRootPath, "Imagenes", "Productos", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // Log error pero no interrumpir el flujo principal
                Console.WriteLine($"Error al eliminar imagen: {ex.Message}");
            }
        }
    }
}
