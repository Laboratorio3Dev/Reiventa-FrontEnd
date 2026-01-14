namespace WebBackOffice.DTO.Oficinas
{
    public class RetencionHipotecariaSolicitudDTO
    {
        public int ID { get; set; }
        public string? PRODUCTO { get; set; }
        public string? DNI_CLIENTE { get; set; }
        public string? MONEDA { get; set; }
        public decimal? TASA_SOLICITADA { get; set; }
        public decimal? SALDO_CREDITO { get; set; }
        public decimal? TASA_OFRECIDA { get; set; }
        public string? ENTIDAD { get; set; }
        public string? ID_USUARIO { get; set; }
        public int? ESTADO { get; set; }
        public DateTime? FECHA_REGISTRO { get; set; }
        public decimal? TASA_RESPUESTA { get; set; }
    }
}
