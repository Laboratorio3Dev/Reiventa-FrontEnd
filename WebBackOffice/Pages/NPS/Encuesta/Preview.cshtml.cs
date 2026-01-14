using System.Text.Json;
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
        public int LLaveEncuesta { get; set; } // 0 cuando entras a /Encuesta/Preview

        public EncuestaResponseDTO? encuesta { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/BackOffice/Login");

            if (LLaveEncuesta > 0)
            {
                encuesta = await _service.ObtenerEncuestaPorId(token, LLaveEncuesta);
                if (encuesta == null) return NotFound();
                return Page();
            }

            // ===== DRAFT desde Session (cuando aún no existe) =====
            encuesta = BuildFromSessionDraft();
            if (encuesta == null)
                return RedirectToPage("/Encuestas/NPS"); // no hay nada que previsualizar

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToPage("/BackOffice/Login");

            // Si existe por ID -> recarga por API
            if (LLaveEncuesta > 0)
            {
                encuesta = await _service.ObtenerEncuestaPorId(token, LLaveEncuesta);
                if (encuesta == null) return NotFound();
                return Page();
            }

            // Si es draft -> recarga desde session
            encuesta = BuildFromSessionDraft();
            if (encuesta == null)
                return RedirectToPage("/Encuestas/NPS");

            return Page();
        }

        private EncuestaResponseDTO? BuildFromSessionDraft()
        {
            var rawForm = HttpContext.Session.GetString("NPS_formModel");
            if (string.IsNullOrWhiteSpace(rawForm))
                return null;

            EncuestaResponseDTO formModel;
            try
            {
                formModel = JsonSerializer.Deserialize<EncuestaResponseDTO>(rawForm) ?? new EncuestaResponseDTO();
            }
            catch
            {
                return null;
            }

            var rawPreg = HttpContext.Session.GetString("NPS_preguntasDisponibles");
            List<PreguntaResponse> preguntas = new();

            if (!string.IsNullOrWhiteSpace(rawPreg))
            {
                try
                {
                    preguntas = JsonSerializer.Deserialize<List<PreguntaResponse>>(rawPreg) ?? new List<PreguntaResponse>();
                }
                catch
                {
                    preguntas = new List<PreguntaResponse>();
                }
            }

            return BuildPreviewEncuesta(formModel, preguntas);
        }

        private static EncuestaResponseDTO BuildPreviewEncuesta(
    EncuestaResponseDTO form,
    List<PreguntaResponse> preguntas)
        {
            form.EncuestaPreguntas = preguntas.Select((p, idx) => new PreguntasEncuesta
            {
                Orden = idx + 1,
                Pregunta = p.Texto,
                TipoPregunta = p.Tipo,

                RangoMinimo = p.RangoMin,
                RangoMaximo = p.RangoMax,

                FlagComentario = p.FlagComentario,
                TextoDetractor = p.TextoDetractor ?? "",
                TextoNeutro = p.TextoNeutro ?? "",
                TextoPromotor = p.TextoPromotor ?? "",
                TextoValorMinimo = p.TextoValorMinimo ?? "",
                TextoValorMaximo = p.TextoValorMaximo ?? "",

                ValoresX = p.Afirmaciones != null
                    ? string.Join("|", p.Afirmaciones)
                    : "",

                ValoresY = p.RespuestasLikert != null
                    ? string.Join("|", p.RespuestasLikert)
                    : ""
            }).ToList();

            if (string.IsNullOrWhiteSpace(form.TituloEncuesta))
                form.TituloEncuesta = form.NombreEncuesta ?? "Vista previa";

            return form;
        }

    }
}
