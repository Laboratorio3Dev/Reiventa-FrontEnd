namespace WebBackOffice.DTO.Ofertas
{
    public class HistorialConsultasRequestDTO
    {
        public string Usuario { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class PaginadoResponse<T>
    {
        public List<T> Registros { get; set; }
        public int TotalRegistros { get; set; }
    }
}
