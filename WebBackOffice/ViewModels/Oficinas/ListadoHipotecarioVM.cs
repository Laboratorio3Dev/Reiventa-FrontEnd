namespace WebBackOffice.ViewModels.Oficinas
{
    public class ListadoHipotecarioVM
    {
        public string Ejecutivo { get; set; }
        public string Documento { get; set; }
        public string Celular { get; set; }
        public string Correo { get; set; }
        public string Score { get; set; }
        public DateTime Fecha { get; set; }

        public string Estado { get; set; }

        // ===== PROPIEDADES DE PRESENTACIÓN =====
        //public string EstadoTexto => Estado switch
        //{
        //    1 => "SIN GESTIÓN",
        //    2 => "INTERESADO",
        //    3 => "EN SEGUIMIENTO",
        //    4 => "RECHAZADO",
        //    5 => "NO CONTACTABLE",
        //    6 => "APROBADO",
        //    7 => "NO INTERESADO",
        //    8 => "OBSERVADO",
        //    _ => "OTRO"
        //};

        //public string ColorEstado => Estado switch
        //{
        //    1 => "bg-danger",
        //    2 => "bg-success",
        //    3 => "bg-secondary",
        //    4 => "bg-dark",
        //    5 => "bg-warning text-dark",
        //    6 => "bg-success",
        //    7 => "bg-warning",
        //    8 => "bg-primary",
        //    _ => "bg-secondary"
        //};
    }

}
