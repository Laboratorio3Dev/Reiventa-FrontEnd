namespace WebBackOffice.DTO.NPS
{
    public class ActualizaEncuestaDTO
    {
        public int? IdEncuesta { get; set; }
        public string? NombreEncuesta { get; set; }
        public string? TituloEncuesta { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool FlagLogin { get; set; }
        public byte[]? ImagenLogin { get; set; }
        public bool FlagBase { get; set; }
    }
}
