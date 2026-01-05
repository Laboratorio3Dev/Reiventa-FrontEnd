namespace WebBackOffice.DTO.NPS
{
    public class PreguntaResponse
    {
        public int TempId { get; set; }
        public int? IdEncuesta { get; set; }
        public int IdPregunta { get; set; }
        public string Texto { get; set; } = string.Empty;
        public string TextoDetractor { get; set; } = string.Empty;
        public string TextoNeutro { get; set; } = string.Empty;
        public string TextoPromotor { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int? RangoMin { get; set; }
        public int? RangoMax { get; set; }
        public bool FlagComentario { get; set; }
        public List<string> Afirmaciones { get; set; } = new();
        public List<string> RespuestasLikert { get; set; } = new();
        public string RespuestaSimple { get; set; } = string.Empty;
        public string? TextoValorMinimo { get; set; }
        public string? TextoValorMaximo { get; set; }
    }
}

