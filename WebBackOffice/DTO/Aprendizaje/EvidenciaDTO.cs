
namespace WebBackOffice.DTO.Aprendizaje
{
    public class EvidenciaDTO
    {
        public byte[] Archivo { get; set; } = Array.Empty<byte>();
        public string Nombre { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
