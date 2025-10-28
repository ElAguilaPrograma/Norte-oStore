namespace WebApplication2.DTO
{
    public class UsuarioRegisterRequest
    {
        public string Nombre { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int ID_Rol { get; set; }
    }
}
