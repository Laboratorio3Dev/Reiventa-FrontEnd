namespace WebBackOffice.DTO.NPS
{
    public class ActualizarEncuestaDTO
    {
        public string Usuario { get; set; }
        public List<PreguntasEliminar> preguntasEliminadas { get; set; }
        public ActualizaEncuestaDTO DatosEncuesta { get; set; }
        public List<ActualizaEncuestaPreguntaDTO> EncuestaPreguntas { get; set; }
    }
}
