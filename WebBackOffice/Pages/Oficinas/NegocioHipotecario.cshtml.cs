using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.ViewModels.Oficinas;

namespace WebBackOffice.Pages.Oficinas
{
    public class NegocioHipotecarioModel : PageModel
    {
        public DashboardResumenVM Resumen { get; set; } = new();
        public List<ListadoHipotecarioVM> Listado { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime FechaInicio { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime FechaFin { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Estado { get; set; }

        public void OnGet()
        {
            // 🔹 Aquí cargas Resumen y Listado desde servicio
        }

        //public async Task<IActionResult> OnGetGestionAsync(string documento)
        //{
        //    var data = new GestionClienteVM(); ; //= await _servicio.ObtenerGestionCliente(documento);

        //    return Partial("_GestionCliente", data);
        //}
     


        public IActionResult OnGetGestion()
        {
            var vm = new GestionClienteVM
            {
                Estado = 1 // Sin Gestión por defecto
            };

            return Partial("_GestionCliente", vm);
        }

    }
}
