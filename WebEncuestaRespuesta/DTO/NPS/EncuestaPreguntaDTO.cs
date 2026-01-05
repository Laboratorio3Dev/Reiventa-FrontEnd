namespace WebEncuestaRespuesta.DTO.NPS
{
    public class EncuestaPreguntaDTO
    {
        public string? Pregunta { get; set; }
        public string? TipoPregunta { get; set; }
        public int? Orden { get; set; }
        public int? RangoMinimo { get; set; }
        public int? RangoMaximo { get; set; }
        public bool FlagComentario { get; set; }
        public string? Comentario { get; set; }
        public string? TextoDetractor { get; set; }
        public string? TextoNeutro { get; set; }
        public string? TextoPromotor { get; set; }
        public string? ValoresX { get; set; }
        public string? ValoresY { get; set; }
        public string? TextoValorMinimo { get; set; }
        public string? TextoValorMaximo { get; set; }
    }
}
