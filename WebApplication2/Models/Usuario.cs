using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Usuario
    {
        public int ID_Usuarios { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;

        public int ID_Rol { get; set; }

        // Relación (opcional, útil si haces joins)
        [ForeignKey("ID_Rol")]
        public Rol? Rol { get; set; }
    }
}
