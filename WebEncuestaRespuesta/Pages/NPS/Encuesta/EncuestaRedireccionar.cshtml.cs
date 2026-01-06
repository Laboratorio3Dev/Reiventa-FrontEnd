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

        
        [BindProperty(SupportsGet = true, Name = "encuesta")]
        public string? EncuestaEncriptada { get; set; }

        [BindProperty(SupportsGet = true, Name = "u")]
        public string? UsuarioEncriptado { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(EncuestaEncriptada))
                return Redirect(Url.Content($"~/NPS/Encuesta/noExiste")); 

            var enc = EncuestaEncriptada.Trim().Replace(" ", "+");

            EncuestaResponseDTO? encuesta;
            try
            {
                encuesta = await _service.ObtenerEncuestaPorIdEncriptado(token: null, enc);
            }
            catch
            {
                return Redirect(Url.Content($"~/NPS/Encuesta/noExiste"));
            }

            if (encuesta == null)
                return Redirect(Url.Content($"~/NPS/Encuesta/noExiste"));

            if (encuesta.FlagLogin == false && encuesta.FlagBase == false)
            {
                return Redirect(Url.Content($"~/NPS/Encuesta/Responder?encuesta={Uri.EscapeDataString(enc)}"));
            }
            else if ((encuesta.FlagLogin || encuesta.FlagBase) && string.IsNullOrWhiteSpace(UsuarioEncriptado))
            {
                return Redirect(Url.Content($"~/NPS/Encuesta/Inicio?encuesta={Uri.EscapeDataString(enc)}"));
            }
            if (!string.IsNullOrWhiteSpace(UsuarioEncriptado))
            {
                return Redirect(Url.Content($"~/NPS/Encuesta/responder?encuesta={Uri.EscapeDataString(enc)}&u={Uri.EscapeDataString(UsuarioEncriptado)}"));
            }
            if (encuesta.FlagLogin)
            {
                return Redirect(Url.Content($"~/NPS/Encuesta/Inicio?encuesta={Uri.EscapeDataString(enc)}"));
            }

            return Redirect(Url.Content($"~/NPS/Encuesta/Inicio?encuesta={Uri.EscapeDataString(enc)}"));
        }
    }
}
