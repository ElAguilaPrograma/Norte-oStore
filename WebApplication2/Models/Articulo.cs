using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Articulo
    {
        public int ID_Articulo { get; set; }
        public string Nombre_Articulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        public int ID_Categoria { get; set; }

        public string ImagenUrl { get; set; }

        // 🔗 Relación opcional
        [ForeignKey("ID_Categoria")]
        public Categoria? Categoria { get; set; }
    }
}
