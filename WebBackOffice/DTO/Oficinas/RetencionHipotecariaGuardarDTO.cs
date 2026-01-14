namespace WebBackOffice.DTO.Oficinas
{
    public class RetencionHipotecariaGuardarDTO
    {
        public int? IdSolicitud { get; set; }
        public string NroPrestamo { get; set; } = null!;
        public decimal TasaSolicitada { get; set; }
        public string Moneda { get; set; } = null!;
        public decimal SaldoCredito { get; set; }
        public int EntidadId { get; set; }
        public decimal? TasaOfrecida { get; set; }
        public int ProductoId { get; set; }
        public string Usuario { get; set; }
    }
}
