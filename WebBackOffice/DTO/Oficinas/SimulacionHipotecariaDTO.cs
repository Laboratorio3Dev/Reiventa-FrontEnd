namespace WebBackOffice.DTO.Oficinas
{
    public class SimulacionHipotecariaDTO
    {
        public string Moneda { get; set; }
        public int ValorInmueble { get; set; }
        public int MontoInicial { get; set; }
        public int DineroNecesita { get; set; }
        public int IngresoMensual { get; set; }
        public string TipoIngreso { get; set; }
        public string ComparteCuota { get; set; }
        public string ConoceInmueble { get; set; }
        public string PrimeraVivienda { get; set; }
    }
}
