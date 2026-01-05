using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebEncuestaRespuesta.DTO.NPS;              // ajusta namespace si tu DTO está en otro proyecto
using WebEncuestaRespuesta.Pages.Repositorios;   // donde vive tu BackOfficeLabService / NPSService

namespace WebEncuestaRespuesta.Pages.NPS.Encuesta
{
    public class EncuestaRedireccionarModel : PageModel
    {
        private readonly NPSService _service; // o el servicio real que uses acá

        public EncuestaRedireccionarModel(NPSService service)
        {
            _service = service;
        }

        // ?encuesta=...&u=...
        [BindProperty(SupportsGet = true, Name = "encuesta")]
        public string? EncuestaEncriptada { get; set; }

        [BindProperty(SupportsGet = true, Name = "u")]
        public string? UsuarioEncriptado { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(EncuestaEncriptada))
                return RedirectToPage("/Error"); // o una página tuya: /NPS/Encuesta/ErrorLink

            // en blazor hacías: Trim + Replace(" ", "+")
            var enc = EncuestaEncriptada.Trim().Replace(" ", "+");

            EncuestaResponseDTO? encuesta;
            try
            {
                encuesta = await _service.ObtenerEncuestaPorIdEncriptado(token: null, enc);
            }
            catch
            {
                // si quieres, manda a una pantalla de "link inválido"
                return RedirectToPage("/Error");
            }

            if (encuesta == null)
                return RedirectToPage("/Error");

            // === Tu misma lógica ===
            // 1) No login y no base => responder directo
            if (encuesta.FlagLogin == false && encuesta.FlagBase == false)
            {
                return Redirect($"/NPS/Encuesta/Responder?encuesta={Uri.EscapeDataString(enc)}");
            }

            // 2) Si viene usuario => responder con usuario
            if (!string.IsNullOrWhiteSpace(UsuarioEncriptado))
            {
                return Redirect($"/NPS/Encuesta/Responder?encuesta={Uri.EscapeDataString(enc)}&u={Uri.EscapeDataString(UsuarioEncriptado)}");
            }

            // 3) Si requiere login => ir a Inicio
            if (encuesta.FlagLogin)
            {
                return Redirect($"/NPS/Encuesta/Inicio?encuesta={Uri.EscapeDataString(enc)}");
            }

            // 4) Caso típico: FlagBase true pero no llegó u
            // aquí normalmente deberías mandar a una pantalla para pedir/validar documento o seleccionar usuario.
            return Redirect($"/NPS/Encuesta/Inicio?encuesta={Uri.EscapeDataString(enc)}");
        }
    }
}
