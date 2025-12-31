using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.DTO.Ofertas;

namespace WebBackOffice.Pages.Ofertas
{
    public class HistorialPLDModel : PageModel
    {
        [BindProperty]
        public string FiltroUsuario { get; set; }

        [BindProperty]
        public DateTime? FechaInicio { get; set; }

        [BindProperty]
        public DateTime? FechaFin { get; set; }

        // RESULTADOS
        public List<HistorialConsultaDto> Resultados { get; set; }

        public bool BusquedaRealizada { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            BusquedaRealizada = true;

            // ⚠️ Simulación de datos
            Resultados = ObtenerDatosSimulados()
                .Where(x =>
                    (string.IsNullOrEmpty(FiltroUsuario) || x.Usuario.Contains(FiltroUsuario)) &&
                    (!FechaInicio.HasValue || x.Fecha.Date >= FechaInicio.Value.Date) &&
                    (!FechaFin.HasValue || x.Fecha.Date <= FechaFin.Value.Date)
                )
                .ToList();
        }

        // SIMULACIÓN (reemplazar por BD o API)
        private List<HistorialConsultaDto> ObtenerDatosSimulados()
        {
            return new List<HistorialConsultaDto>
        {
            new() {
                Documento = "12345678",
                Usuario = "cvillavicencio",
                Fecha = DateTime.Now.AddDays(-1),
                EsCliente = true
            },
            new() {
                Documento = "87654321",
                Usuario = "jlopez",
                Fecha = DateTime.Now.AddDays(-2),
                EsCliente = false
            }
        };
        }
    }
}