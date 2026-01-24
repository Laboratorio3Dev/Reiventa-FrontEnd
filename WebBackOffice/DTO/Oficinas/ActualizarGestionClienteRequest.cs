namespace WebBackOffice.DTO.Oficinas
{
    public class ActualizarGestionClienteRequest
    {
        public int Id { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
    }
}
