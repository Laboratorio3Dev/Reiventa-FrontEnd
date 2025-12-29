namespace WebBackOffice.DTO.BackOffice
{
    public class LogAuditoria
    {
        public string? CodigoUnico { get; set; }
        public int? Paso { get; set; }
        public string? DetalleLog { get; set; }
        public string? IpCliente { get; set; }
        public string? Utm { get; set; }
        public string? Source { get; set; }
        public string? Medium { get; set; }
        public string? Campaign { get; set; }
        public string? SistemaOperativo { get; set; }
    }
}
