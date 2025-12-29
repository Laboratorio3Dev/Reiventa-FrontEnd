namespace WebBackOffice.DTO.Aprendizaje
{
    public class PlanAccionDTO
    {
        public int ID_PLANACCION { get; set; }
        public string? PRODUCTO { get; set; }
        public string? DIMENSION { get; set; }
        public string? TAREA { get; set; }
        public string? USUARIO { get; set; }
        public string? FECHA { get; set; }
        public string? ESTADO { get; set; }
        public string? Comentario { get; set; }
        public string? Oficina { get; set; }
        public string? Zona { get; set; }
        public string? GerenteOficina { get; set; }
        public EvidenciaDTO? Evidencia { get; set; }

        public string EstadoCss => ESTADO switch
        {
            "Completada" => "badge-ok",
            "Revisión GO" => "badge-revision",
            "Atrasada" => "badge-atrasada",
            "No Iniciada" => "badge-no",
            "Observada" => "badge-observada",
            _ => "badge-proceso" // En Proceso
        };
    }


}
