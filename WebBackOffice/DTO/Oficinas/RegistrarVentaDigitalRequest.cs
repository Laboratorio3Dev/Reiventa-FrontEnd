namespace WebBackOffice.DTO.Oficinas
{
    public class RegistrarVentaDigitalRequest
    {
        public List<int> ProductosSeleccionados { get; set; } = new();

        public string CorreoCliente { get; set; }
        public string UsuarioRegistro { get; set; }

        public string DocumentoCliente { get; set; }
        public string NombreCliente { get; set; }
        public string CodigoVendedor { get; set; }
        public string CodOficina { get; set; }
    }
}
