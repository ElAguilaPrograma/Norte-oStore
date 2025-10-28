namespace WebApplication2.DTO
{
    public class ProductoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;

        public IFormFile Imagen { get; set; }
    }
}
