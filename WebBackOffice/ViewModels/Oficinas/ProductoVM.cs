namespace WebBackOffice.ViewModels.Oficinas
{
    public class ProductoVM
    {
        public int IdProducto { get; set; }
        public string Titulo { get; set; }
        public string SubTitulo { get; set; }
        
        // HTML del correo (editor)
        public string HtmlCorreo { get; set; }

        public string Asunto { get; set; }

        public bool Seleccionado { get; set; }
    }
}
