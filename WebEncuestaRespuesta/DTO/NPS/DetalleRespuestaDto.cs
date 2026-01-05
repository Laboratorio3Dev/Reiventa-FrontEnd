namespace WebEncuestaRespuesta.DTO.NPS
{
    public class DetalleRespuestaDto
    {
        public int IdPregunta { get; set; }
        public string? ValorRespuesta { get; set; }
        public string? ValorComentario { get; set; }
        public string? PalabraClave { get; set; }
    }
}
