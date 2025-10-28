using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Carrito
    {
        public int ID_Carrito_Item { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Articulo { get; set; }
        public int Cantidad { get; set; }
        public DateTime? Fecha_Agregado { get; set; }

        // 🔗 Relaciones opcionales (útiles en consultas con JOIN)
        [ForeignKey("ID_Usuario")]
        public Usuario? Usuario { get; set; }

        [ForeignKey("ID_Articulo")]
        public Articulo? Articulo { get; set; }
    }
}
