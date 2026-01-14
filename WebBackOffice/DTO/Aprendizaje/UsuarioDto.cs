namespace WebBackOffice.DTO.Aprendizaje
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string UsuarioWindows { get; set; }
        public string? Oficina { get; set; }
        public string Rol { get; set; }
        public string? Zona { get; set; }
        public int Estado { get; set; }

    }
}
