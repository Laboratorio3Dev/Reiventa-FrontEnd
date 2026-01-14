namespace WebBackOffice.DTO.Oficinas
{
    public class ListarRetencionHipotecariaRequest
    {
        public string Usuario { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
