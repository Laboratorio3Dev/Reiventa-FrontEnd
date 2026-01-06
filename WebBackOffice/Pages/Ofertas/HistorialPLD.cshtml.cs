using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using WebBackOffice.DTO.Ofertas;
using WebBackOffice.Pages.Repositorios;

namespace WebBackOffice.Pages.Ofertas
{
    public class HistorialPLDModel : PageModel
    {
        private readonly BackOfficeLabService _service;

        public HistorialPLDModel(
            BackOfficeLabService service)
        {
            _service = service;
        }

        [BindProperty]
        public string FiltroUsuario { get; set; }

        [BindProperty]
        public DateTime? FechaInicioCombo { get; set; }

        [BindProperty]
        public DateTime? FechaFinCombo { get; set; }


        // 🔹 PAGINACIÓN (MISMO ESTILO QUE PLAN ACCIÓN)
        [BindProperty(SupportsGet = true, Name = "p")]
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRegistros { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / PageSize);


        // RESULTADOS
        public List<HistorialConsultaDto> Resultados { get; set; }

        public bool BusquedaRealizada { get; set; }

        public async Task OnGetAsync()
        {
            var hoy = DateTime.Today;
            FechaInicioCombo ??= new DateTime(hoy.Year, hoy.Month, 1);
            FechaFinCombo ??= hoy;

            await CargarHistorial();
        }
        private async Task CargarHistorial()
        {
            var request = new HistorialConsultasRequestDTO
            {
                Usuario = FiltroUsuario,
                FechaInicio = FechaInicioCombo?.ToString("yyyyMMdd"),
                FechaFin = FechaFinCombo?.ToString("yyyyMMdd"),
                PageNumber = Page,
                PageSize = PageSize
            };

            var token = HttpContext.Session.GetString("Token");

            var response = await _service.ListarHistorialSolicitudesPLD(token, request);
            response = response
              .Where(x =>
                  (string.IsNullOrEmpty(FiltroUsuario) || x.Usuario.ToUpper().Contains(FiltroUsuario.ToUpper()))
              )
              .ToList();

            Resultados = response;
            TotalRegistros = response.Count();
        }
        public async Task OnPostAsync()
        {
            Page = 1; // 🔥 igual que PlanAccion
            await CargarHistorial();
           
        }
       



    }
}