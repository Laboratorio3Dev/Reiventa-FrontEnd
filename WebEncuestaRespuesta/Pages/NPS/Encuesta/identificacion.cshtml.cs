using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebEncuestaRespuesta.DTO.NPS;
using WebEncuestaRespuesta.Pages.Repositorios;

namespace WebEncuestaRespuesta.Pages.NPS.Encuesta
{
    public class identificacionModel : PageModel
    {
        private readonly NPSService ServiceRepositorio;

        public identificacionModel(NPSService serviceRepositorio)
        {
            ServiceRepositorio = serviceRepositorio;
        }

        [BindProperty(SupportsGet = true, Name = "encuesta")]
        public string? encuestaEnctriptada { get; set; }

        // Inputs del formulario
        [BindProperty]
        public string Dni { get; set; } = string.Empty;

        [BindProperty]
        public bool Condicion1 { get; set; }

        [BindProperty]
        public bool Condicion2 { get; set; }

        // Errores
        public string? Error { get; set; }
        public string? ErrorDni { get; set; }
        public bool MostrarErrorCondicion1 { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrWhiteSpace(encuestaEnctriptada))
            {
                Error = "Token de encuesta inválido.";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            encuestaEnctriptada = Request.Form["encuesta"];

            if (string.IsNullOrWhiteSpace(encuestaEnctriptada))
            {
                Error = "Token de encuesta inválido.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Dni) || Dni.Length != 8 || !Dni.All(char.IsDigit))
            {
                ErrorDni = "Ingresa un DNI válido (8 dígitos).";
                return Page();
            }

            if (!Condicion1)
            {
                MostrarErrorCondicion1 = true;
                Error = "Debes aceptar la cláusula obligatoria para continuar.";
                return Page();
            }

            var encuestaToken = encuestaEnctriptada.Trim().Replace(" ", "+");

            var body = new ValidarClienteRequest
            {
                NroDocumento = Dni
            };

            var usuarioToken = await ServiceRepositorio.obtenerURLClientePorDni(body, encuestaToken);

            if (string.IsNullOrWhiteSpace(usuarioToken))
            {
                Error = "No se encontró información para el documento ingresado o no está habilitado para esta encuesta.";
                return Page();
            }

            usuarioToken = usuarioToken.Trim().Replace(" ", "+");
            return Redirect(Url.Content($"~/NPS/Encuesta/responder?encuesta={Uri.EscapeDataString(encuestaToken)}&u={usuarioToken}"));
        }
    }
}