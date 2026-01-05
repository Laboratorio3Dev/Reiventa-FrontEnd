namespace WebBackOffice.DTO.Aprendizaje
{
    public class ResponseTransacciones
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int IdValue { get; set; }
        public object? Data { get; set; }
    }
}
