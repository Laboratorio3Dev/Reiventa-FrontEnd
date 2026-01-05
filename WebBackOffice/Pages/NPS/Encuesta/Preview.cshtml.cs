using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.DTO.NPS;
using WebBackOffice.Pages.Repositorios;
namespace WebBackOffice.Pages.NPS.Encuesta
{
    public class PreviewModel : PageModel
    {
        private readonly NPSService _service;

        public PreviewModel(NPSService service)
        {
            _service = service;
        }

        [FromRoute]
        public int LLaveEncuesta { get; set; }

        public EncuestaResponseDTO? encuesta { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/BackOffice/Login");

            encuesta = await _service.ObtenerEncuestaPorId(token, LLaveEncuesta);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Por ahora solo “vista previa”: no guarda nada, solo recarga la encuesta.
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/BackOffice/Login");

            encuesta = await _service.ObtenerEncuestaPorId(token, LLaveEncuesta);
            return Page();
        }
    }
}
