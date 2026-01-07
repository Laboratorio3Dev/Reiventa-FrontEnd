using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebEncuestaRespuesta.DTO.NPS;
using System.Text.RegularExpressions;
using WebEncuestaRespuesta.Pages.Repositorios;

namespace WebBackOffice.Pages.NPS.Encuesta
{
    public class ResponderEncuestaModel : PageModel
    {
        private readonly NPSService ServiceRepositorio;

        public ResponderEncuestaModel(NPSService serviceRepositorio)
        {
            ServiceRepositorio = serviceRepositorio;
        }

        [BindProperty(SupportsGet = true, Name = "encuesta")]
        public string? encuestaEnctriptada { get; set; }

        [BindProperty(SupportsGet = true, Name = "u")]
        public string? usuarioEncriptado { get; set; }

        // Si lo usas, mantenlo, pero en tu Blazor no era realmente necesario para la página
        [BindProperty]
        public int LLaveEncuesta { get; set; }

        public EncuestaResponseDTO? Encuesta { get; set; }

        public bool Guardando { get; set; }
        public string? ErrorCarga { get; set; }
        public string? ErrorGuardar { get; set; }

        // Prefills opcionales si quieres re-renderizar con errores sin perder datos
        public Dictionary<int, int> PrefillRespuesta { get; } = new();
        public Dictionary<int, string> PrefillComentario { get; } = new();
        public Dictionary<string, int> PrefillLikert { get; } = new();
        public Dictionary<int, string> PrefillTexto { get; } = new();
        public Dictionary<int, string> PrefillLikertComentario { get; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(encuestaEnctriptada))
            {
                ErrorCarga = "Token de encuesta inválido.";
                return Page();
            }

            try
            {
                Encuesta = await ServiceRepositorio.ObtenerEncuestaPorIdEncriptado(".", encuestaEnctriptada);
            }
            catch (Exception ex)
            {
                ErrorCarga = $"Error al obtener encuesta: {ex.Message}";
                return Page();
            }

            if (Encuesta == null)
                return Redirect(Url.Content("~/NPS/Encuesta/noExiste"));

           if ((Encuesta.FlagLogin || Encuesta.FlagBase) && string.IsNullOrWhiteSpace(usuarioEncriptado))
            {
                return Redirect(Url.Content($"~/NPS/Encuesta/Inicio?encuesta={Uri.EscapeDataString(encuestaEnctriptada)}"));
            }

            if ((Encuesta.FlagLogin || Encuesta.FlagBase) && !string.IsNullOrWhiteSpace(usuarioEncriptado))
            {
               var existe = await ServiceRepositorio.validarClienteEncuesta(usuarioEncriptado, encuestaEnctriptada);
                if (existe.Existe && existe.YaRespondio)
                    return Redirect(Url.Content($"~/NPS/Encuesta/Agradecimiento"));
                else if (!existe.Existe)
                {
                    return Redirect(Url.Content($"~/NPS/Encuesta/noIdentificado"));
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Guardando = true;
            ErrorGuardar = null;

            // Recupera hidden fields
            encuestaEnctriptada = Request.Form["encuesta"];
            usuarioEncriptado = Request.Form["u"];

            if (string.IsNullOrWhiteSpace(encuestaEnctriptada))
            {
                ErrorGuardar = "Token de encuesta inválido.";
                Guardando = false;
                return Page();
            }

            // Carga encuesta nuevamente (stateless)
            try
            {
                Encuesta = await ServiceRepositorio.ObtenerEncuestaPorIdEncriptado(".", encuestaEnctriptada);
            }
            catch (Exception ex)
            {
                ErrorGuardar = ex.Message;
                Guardando = false;
                return Page();
            }

            if (Encuesta is null)
            {
                ErrorGuardar = "No se cargó la encuesta.";
                Guardando = false;
                return Page();
            }

            var encuestaToken = encuestaEnctriptada.Trim().Replace(" ", "+");
            var usuarioToken = (usuarioEncriptado ?? "").Trim().Replace(" ", "+");

            if (Encuesta.FlagLogin && string.IsNullOrWhiteSpace(usuarioToken))
                return Redirect(Url.Content($"~/NPS/Encuesta/Inicio?encuesta={Uri.EscapeDataString(encuestaToken)}"));

            if (Encuesta.FlagBase && string.IsNullOrWhiteSpace(usuarioToken))
                return Redirect(Url.Content($"~/NPS/Encuesta/Inicio?encuesta={Uri.EscapeDataString(encuestaToken)}"));

            if ((Encuesta.FlagBase || Encuesta.FlagLogin) && !string.IsNullOrWhiteSpace(usuarioToken))
            {
                var existe = await ServiceRepositorio.validarClienteEncuesta(usuarioToken, encuestaToken);
                if (existe.Existe && existe.YaRespondio)
                    return Redirect(Url.Content($"~/NPS/Encuesta/Agradecimiento"));
            }


            var respuesta = ParseIntDictionaryFromForm("Respuesta");
            var comentario = ParseStringDictionaryFromForm("Comentario");
            var textoLibre = ParseStringDictionaryFromForm("Texto");
            var likert = ParseLikertFromForm("Likert");

            foreach (var kv in respuesta) PrefillRespuesta[kv.Key] = kv.Value;
            foreach (var kv in comentario) PrefillComentario[kv.Key] = kv.Value ?? "";
            foreach (var kv in textoLibre) PrefillTexto[kv.Key] = kv.Value ?? "";
            foreach (var kv in likert) PrefillLikert[kv.Key] = kv.Value;

            var detalles = new List<DetalleRespuestaDto>();

            foreach (var preg in Encuesta.EncuestaPreguntas.OrderBy(x => x.Orden))
            {
                var orden = preg.Orden!.Value;
                var idPregunta = preg.IdEncuestaPregunta;

                if (preg.TipoPregunta == "Escala Numérica")
                {
                    if (!respuesta.TryGetValue(orden, out var valor))
                    {
                        ErrorGuardar = $"Falta responder la pregunta {orden}.";
                        Guardando = false;
                        return Page();
                    }

                    detalles.Add(new DetalleRespuestaDto
                    {
                        IdPregunta = idPregunta,
                        ValorRespuesta = valor.ToString(),
                        ValorComentario = comentario.GetValueOrDefault(orden)
                    });
                }
                else if (preg.TipoPregunta == "Escala de Likert")
                {
                    var valoresY = (preg.ValoresY ?? "")
                        .Split('|', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();

                    var respuestasOrdenadas = new List<string>();

                    foreach (var y in valoresY)
                    {
                        var key = $"{orden}|{y}";

                        if (!likert.TryGetValue(key, out var valorLikert))
                        {
                            ErrorGuardar = $"Falta responder Likert ({orden}) en: {y}";
                            Guardando = false;
                            return Page();
                        }

                        respuestasOrdenadas.Add(valorLikert.ToString());
                    }

                    var valorConcatenado = string.Join("|", respuestasOrdenadas);

                    detalles.Add(new DetalleRespuestaDto
                    {
                        IdPregunta = idPregunta,
                        ValorRespuesta = valorConcatenado,
                        ValorComentario = null,
                        PalabraClave = null
                    });
                }
                else if (preg.TipoPregunta == "Pregunta")
                {
                    if (!textoLibre.TryGetValue(orden, out var texto) || string.IsNullOrWhiteSpace(texto))
                    {
                        ErrorGuardar = $"Falta responder la pregunta {orden}.";
                        Guardando = false;
                        return Page();
                    }

                    detalles.Add(new DetalleRespuestaDto
                    {
                        IdPregunta = idPregunta,
                        ValorComentario = texto
                    });
                }
            }

            var request = new GuardarRespuestasRequest
            {
                EncuestaToken = encuestaToken,
                UsuarioToken = usuarioToken,
                Detalles = detalles
            };

            try
            {
                var ok = await ServiceRepositorio.GuardarRespuestas(request);
                if (!ok)
                {
                    ErrorGuardar = "No se pudo guardar la encuesta.";
                    Guardando = false;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorGuardar = ex.Message;
                Guardando = false;
                return Page();
            }

            return Redirect(Url.Content($"~/NPS/Encuesta/Agradecimiento"));
        }

        // ======== Helpers de parseo (sin tocar tus DTOs) ========

        private Dictionary<int, int> ParseIntDictionaryFromForm(string prefix)
        {
            // matches: Respuesta[3] = "10"
            var dict = new Dictionary<int, int>();
            var rx = new Regex($"^{Regex.Escape(prefix)}\\[(\\d+)\\]$");

            foreach (var key in Request.Form.Keys)
            {
                var m = rx.Match(key);
                if (!m.Success) continue;

                if (int.TryParse(m.Groups[1].Value, out var idx) &&
                    int.TryParse(Request.Form[key], out var val))
                {
                    dict[idx] = val;
                }
            }
            return dict;
        }

        private Dictionary<int, string> ParseStringDictionaryFromForm(string prefix)
        {
            var dict = new Dictionary<int, string>();
            var rx = new Regex($"^{Regex.Escape(prefix)}\\[(\\d+)\\]$");

            foreach (var key in Request.Form.Keys)
            {
                var m = rx.Match(key);
                if (!m.Success) continue;

                if (int.TryParse(m.Groups[1].Value, out var idx))
                    dict[idx] = Request.Form[key].ToString();
            }
            return dict;
        }

        private Dictionary<string, int> ParseLikertFromForm(string prefix)
        {
            // matches: Likert[3|Calidad] = "2"
            var dict = new Dictionary<string, int>();
            var rx = new Regex($"^{Regex.Escape(prefix)}\\[(.+)\\]$");

            foreach (var key in Request.Form.Keys)
            {
                var m = rx.Match(key);
                if (!m.Success) continue;

                var compoundKey = m.Groups[1].Value; // "3|Calidad"
                if (int.TryParse(Request.Form[key], out var val))
                    dict[compoundKey] = val;
            }
            return dict;
        }
    }
}
