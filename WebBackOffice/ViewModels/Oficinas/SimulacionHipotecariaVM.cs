namespace WebBackOffice.ViewModels.Oficinas
{
    public class SimulacionHipotecariaVM
    {
        public string Moneda { get; set; }
        public decimal ValorInmueble { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal DineroNecesita { get; set; }
        public decimal IngresoMensual { get; set; }
        public string TipoIngreso { get; set; }
        public string ComparteCuota { get; set; }
        public string ConoceInmueble { get; set; }
        public string PrimeraVivienda { get; set; }
    }

}
