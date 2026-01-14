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
        public EncuestaResponseDTO? Encuesta { get; set; }
        [BindProperty]
        public string LoginBgUrl { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(encuestaEnctriptada))
                return Redirect(Url.Content("~/NPS/Encuesta/noExiste"));

            var token = Uri.UnescapeDataString(encuestaEnctriptada).Trim().Replace(" ", "+");

            Encuesta = await ServiceRepositorio.ObtenerEncuestaPorIdEncriptado(".", token);
            if (Encuesta == null)
                return Redirect(Url.Content("~/NPS/Encuesta/noExiste"));

            if (Encuesta.ImagenLogin != null && Encuesta.ImagenLogin.Length > 0)
            {
                LoginBgUrl = Url.Page("/NPS/Encuesta/identificacion", "LoginBg", new
                {
                    encuesta = encuestaEnctriptada,
                    v = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                }) ?? "";
            }

            return Page();
        }
        public async Task<IActionResult> OnGetLoginBgAsync(string encuesta)
        {
            if (string.IsNullOrWhiteSpace(encuesta))
                return NotFound();

            var token = Uri.UnescapeDataString(encuesta).Trim().Replace(" ", "+");

            var enc = await ServiceRepositorio.ObtenerEncuestaPorIdEncriptado(".", token);
            var bytes = enc?.ImagenLogin;

            if (bytes == null || bytes.Length == 0)
                return NotFound();

            return File(bytes, GetImageContentType(bytes));
        }

        private static string GetImageContentType(byte[] bytes)
        {
            if (bytes.Length >= 8 &&
                bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                return "image/png";

            if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8)
                return "image/jpeg";

            if (bytes.Length >= 3 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                return "image/gif";

            return "application/octet-stream";
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