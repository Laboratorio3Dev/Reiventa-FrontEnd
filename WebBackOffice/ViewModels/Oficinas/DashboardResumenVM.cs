namespace WebBackOffice.ViewModels.Oficinas
{
    public class DashboardResumenVM
    {
        public int Total { get; set; }
        public double PorcentajeGestion { get; set; }
        public double PorcentajeContactabilidad { get; set; }
        public double PorcentajeAprobacion { get; set; }

        public int SinGestion { get; set; }
        public int Interesados { get; set; }
        public int EnSeguimiento { get; set; }
        public int Rechazado { get; set; }
        public int NoContactable { get; set; }
        public int Aprobado { get; set; }
        public int NoInteresado { get; set; }
        public int Observado { get; set; }
    }
}
