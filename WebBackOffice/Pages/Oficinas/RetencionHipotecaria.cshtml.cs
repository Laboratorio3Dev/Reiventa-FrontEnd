using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.Pages.Repositorios;

namespace WebBackOffice.Pages.Oficinas
{
    public class RetencionHipotecariaModel : PageModel
    {
        private readonly RetencionHipotecariaService _service;

        public RetencionHipotecariaModel(RetencionHipotecariaService service)
        {
            _service = service;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnGetListarAsync(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int page = 1)
        {
            var usuario = HttpContext.Session.GetString("Usuario");

            var result = await _service.Listar(
                usuario,
                fechaInicio,
                fechaFin,
                page,
                pageSize: 10
            );

            return new JsonResult(result);
        }
    }
}
