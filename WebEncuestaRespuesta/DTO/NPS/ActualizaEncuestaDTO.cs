namespace WebEncuestaRespuesta.DTO.NPS
{
    public class ActualizaEncuestaDTO
    {
        public int? IdEncuesta { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool FlagLogin { get; set; }
        public byte[]? ImagenLogin { get; set; }
        public bool FlagBase { get; set; }
        public string? TituloEncuesta { get; set; }
    }
}
