namespace WebBackOffice.ViewModels.Oficinas
{
    public class GestionClienteVM
    {
        // Datos cliente
        public int Id { get; set; }
        public string Documento { get; set; }
        public string Celular { get; set; }
        public string CanalContacto { get; set; }   // "NO CONTACTADO WHATSAPP"

        // Gestión
        public string Estado { get; set; }
        public string Comentarios { get; set; }

        // Simulaciones
        public List<SimulacionHipotecariaVM> Simulaciones { get; set; } = new();
      
    }

}
