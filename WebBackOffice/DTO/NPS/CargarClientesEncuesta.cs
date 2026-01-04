namespace WebBackOffice.DTO.NPS
{
    public class CargarClientesEncuesta
    {
        public int IdEncuesta { get; set; }
        public string Usuario { get; set; }
        public List<ClienteEncuestaDto> NPS_ClienteEncuesta { get; set; }
    }
}
