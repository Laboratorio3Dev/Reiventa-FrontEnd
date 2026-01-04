namespace WebBackOffice.DTO.NPS
{
    public class CrearEncuestaDTO
    {
        public string Usuario { get; set; }
        public EncuestaDTO DatosEncuesta { get; set; }
        public List<EncuestaPreguntaDTO> EncuestaPreguntas { get; set; }
        public List<ClienteEncuestaDto> NPS_ClienteEncuesta { get; set; }
    }
}
