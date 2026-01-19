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
        public bool ComparteCuota { get; set; }
        public bool ConoceInmueble { get; set; }
        public bool PrimeraVivienda { get; set; }
    }

}
