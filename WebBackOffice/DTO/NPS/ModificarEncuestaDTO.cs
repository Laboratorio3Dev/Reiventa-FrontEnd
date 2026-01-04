namespace WebBackOffice.DTO.NPS
{
    public class ModificarEncuestaDTO
    {
        public string Usuario { get; set; }
        public List<PreguntasEliminar> preguntasEliminadas { get; set; }
        public ActualizaEncuestaDTO DatosEncuesta { get; set; }
        public List<ActualizaEncuestaPreguntaDTO> EncuestaPreguntas { get; set; }
        public List<ClienteEncuestaDto> NPS_ClienteEncuesta { get; set; }
    }
}
