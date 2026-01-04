namespace WebEncuestaRespuesta.DTO.NPS
{
    public class ExportRespuestasEncuestaDto
    {
        public List<ExportColumnDto> Columns { get; set; } = new();
        public List<Dictionary<string, string?>> Rows { get; set; } = new();
    }
}
