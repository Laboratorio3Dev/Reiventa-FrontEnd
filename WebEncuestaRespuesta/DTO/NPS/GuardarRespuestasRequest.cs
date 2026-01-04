namespace WebEncuestaRespuesta.DTO.NPS
{
    public class GuardarRespuestasRequest
    {
        public string EncuestaToken { get; set; } = "";
        public string UsuarioToken { get; set; } = "";
        public List<DetalleRespuestaDto> Detalles { get; set; } = new();
    }
}
