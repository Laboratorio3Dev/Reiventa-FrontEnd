namespace WebBackOffice.DTO.Oficinas
{
    public class ProductoResponseDTO
    {
        public int IdProducto { get; set; }
        public string? Titulo { get; set; }
        public string? Asunto { get; set; }
        public string? SubTitulo { get; set; }
        public int? Orden { get; set; }
        public bool Activo { get; set; }
    }
}
