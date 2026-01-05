namespace WebBackOffice.DTO.NPS
{
    public class EncuestaDTO
    {
        public string? NombreEncuesta { get; set; }
        public string? TipoPersona { get; set; }
        public string? Link { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool FlagLogin { get; set; }
        public string? UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public byte[]? ImagenLogin { get; set; }
        public bool FlagBase { get; set; }
        public bool FlagAnalisis { get; set; }
        public string? TituloEncuesta { get; set; }
    }
}
