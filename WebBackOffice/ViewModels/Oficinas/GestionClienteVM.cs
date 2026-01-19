namespace WebBackOffice.ViewModels.Oficinas
{
    public class GestionClienteVM
    {
        // Datos cliente
        public string Documento { get; set; }
        public string Celular { get; set; }
        public string CanalContacto { get; set; }   // "NO CONTACTADO WHATSAPP"

        // Gestión
        public int Estado { get; set; }
        public string Comentarios { get; set; }

        // Simulaciones
        public List<SimulacionHipotecariaVM> Simulaciones { get; set; } = new();
        // Opcional: para mostrar texto
        public string EstadoTexto => Estado switch
        {
            1 => "Sin Gestión",
            2 => "Interesado",
            3 => "No Interesado",
            4 => "En Seguimiento",
            5 => "Observado",
            6 => "Rechazado",
            7 => "No Contactable",
            8 => "Aprobado",
            _ => "No definido"
        };
    }

}
