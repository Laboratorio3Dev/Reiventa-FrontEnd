namespace WebBackOffice.DTO.Oficinas
{
    public class ListarNegocioHipotecarioRequest
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Estado { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Usuario { get; set; }
        public string NivelAcceso { get; set; }

    }
}
