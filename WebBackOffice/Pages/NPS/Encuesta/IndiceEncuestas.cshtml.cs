using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel;
using System.Drawing;
using System.Text.Json;
using WebBackOffice.DTO.NPS;
using WebBackOffice.Pages.Repositorios;

namespace WebBackOffice.Pages.NPS.Encuesta
{
    public class IndiceEncuestaModel : PageModel
    {
        private readonly NPSService ServiceRepositorio;
        public IndiceEncuestaModel(NPSService serviceRepositorio)
        {
            ServiceRepositorio = serviceRepositorio;
        }

        public List<EncuestaResponseDTO> encuestas { get; set; } = new();
        public List<EncuestaResponseDTO> pagedEncuestas { get; set; } = new();
        public List<PreguntaResponse> preguntasDisponibles { get; set; } = new();
        public List<PreguntasEliminar> preguntasEliminadas { get; set; } = new();
        public List<int> preguntasSeleccionadas { get; set; } = new();
        [BindProperty]
        public EncuestaResponseDTO formModel { get; set; } = new();
        public PreguntaResponse? preguntaEnEdicion { get; set; }
        public List<ClienteEncuestaDto> clientesEncuesta { get; set; } = new();
        public List<BaseClientesEncuestaDTO> BaseclientesEncuesta { get; set; } = new();
        public string url { get; set; } = string.Empty;

        public bool isLoading { get; set; } = false;
        public bool tieneRespuestas { get; set; } = false;
        public bool mostrarModal { get; set; } = false;
        public bool mostrarModalPregunta { get; set; } = false;
        public bool esEdicion { get; set; } = false;

        [BindProperty(SupportsGet = true)] public string SearchTerm { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public string SelectedEstado { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public string SelectedTipoPersona { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public DateTime? FechaInicioFiltro { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? FechaFinFiltro { get; set; }

        [BindProperty(SupportsGet = true)] public int currentPage { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public int totalPages { get; set; } = 1;

        public string modoModal { get; set; } = "nuevo";
        public bool EsSoloLectura => modoModal == "ver";
        public bool EncuestaConRespuestas { get; set; } = false;

        // Pregunta modal
        [BindProperty] public string nuevaPreguntaTexto { get; set; } = string.Empty;
        [BindProperty] public string nuevaPreguntaTipo { get; set; } = string.Empty;
        [BindProperty]
        public int? rangoMinimo { get; set; }
        [BindProperty]
        public int? rangoMaximo { get; set; }
        [BindProperty] public string textoDetractor { get; set; } = string.Empty;
        [BindProperty] public string textoNeutro { get; set; } = string.Empty;
        [BindProperty] public string textoPromotor { get; set; } = string.Empty;
        [BindProperty] public string textoValorMinimo { get; set; } = string.Empty;
        [BindProperty] public string textoValorMaximo { get; set; } = string.Empty;

        [BindProperty] public string nuevaAfirmacion { get; set; } = string.Empty;
        public List<string> afirmaciones { get; set; } = new();

        [BindProperty] public string nuevaRespuestaLikert { get; set; } = string.Empty;
        public List<string> respuestasLikert { get; set; } = new();

        public bool esEdicionPregunta { get; set; } = false;
        [BindProperty] public string respuestaPreguntaSimple { get; set; } = string.Empty;

        public bool mostrarModalEstadisticas { get; set; } = false;
        public List<string> estadisticasLabels { get; set; } = new() { "Pregunta 1", "Pregunta 2", "Pregunta 3" };
        public List<int> estadisticasValores { get; set; } = new() { 15, 22, 9 };

        public string qrUrlGenerado { get; set; } = string.Empty;
        public bool mostrarQRModal { get; set; } = false;
        public bool mostrarModalVistaPrevia { get; set; } = false;
        private const string SessFormModel = "NPS_formModel";
        private const string SessImgDraft = "NPS_ImagenLoginDraft";
        public string modalTitle { get; set; } = string.Empty;
        public string modalMessage { get; set; } = string.Empty;

        public int? IdPreguntaEditando { get; set; } = null;
        public int IdEncuestaSeleccionada { get; set; } = 0;

        // Archivos (migración de InputFile)
        [BindProperty] public IFormFile? ImagenLoginFile { get; set; }
        [BindProperty] public IFormFile? BaseFile { get; set; }

        // Session equivalents
        public string? token { get; set; }
        public string usuarioLogeado { get; set; } = string.Empty;

        public bool CanGoPrevious => currentPage > 1;
        public bool CanGoNext => currentPage < totalPages;

        // ========== Helpers Session JSON ==========
        private T GetSession<T>(string key, T fallback)
        {
            var raw = HttpContext.Session.GetString(key);
            if (string.IsNullOrWhiteSpace(raw)) return fallback;
            try { return JsonSerializer.Deserialize<T>(raw) ?? fallback; }
            catch { return fallback; }
        }
        private void SetSession<T>(string key, T value)
            => HttpContext.Session.SetString(key, JsonSerializer.Serialize(value));

        private void RequireTokenOrRedirect()
        {
            usuarioLogeado = HttpContext.Session.GetString("Usuario") ?? "";
            token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("NO_TOKEN");
        }

        // ========== GET ==========
        public async Task<IActionResult> OnGetAsync(CancellationToken ct)
        {
            try
            {
                usuarioLogeado = HttpContext.Session.GetString("Usuario") ?? "";
                token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrWhiteSpace(token))
                    return Redirect("/");

                encuestas = await ServiceRepositorio.ObtenerEncuestas(token, "");
                ApplyFilters();

                // Rehidratar estado de modales si estaban abiertos
                mostrarModal = GetSession("NPS_mostrarModal", false);
                modoModal = GetSession("NPS_modoModal", "nuevo");
                EncuestaConRespuestas = GetSession("NPS_EncuestaConRespuestas", false);
                esEdicion = GetSession("NPS_esEdicion", false);

                mostrarModalPregunta = GetSession("NPS_mostrarModalPregunta", false);
                esEdicionPregunta = GetSession("NPS_esEdicionPregunta", false);

                mostrarModalEstadisticas = GetSession("NPS_mostrarModalEstadisticas", false);
                mostrarQRModal = GetSession("NPS_mostrarQRModal", false);
                mostrarModalVistaPrevia = GetSession("NPS_mostrarModalVistaPrevia", false);

                IdEncuestaSeleccionada = GetSession("NPS_IdEncuestaSeleccionada", 0);
                qrUrlGenerado = GetSession("NPS_qrUrlGenerado", "");

                // Estado complejo
                formModel = GetSession("NPS_formModel", new EncuestaResponseDTO());
                preguntasDisponibles = GetSession("NPS_preguntasDisponibles", new List<PreguntaResponse>());
                preguntasEliminadas = GetSession("NPS_preguntasEliminadas", new List<PreguntasEliminar>());
                clientesEncuesta = GetSession("NPS_clientesEncuesta", new List<ClienteEncuestaDto>());

                afirmaciones = GetSession("NPS_afirmaciones", new List<string>());
                respuestasLikert = GetSession("NPS_respuestasLikert", new List<string>());

                IdPreguntaEditando = GetSession<int?>("NPS_IdPreguntaEditando", null);

                // campos pregunta modal
                nuevaPreguntaTexto = GetSession("NPS_nuevaPreguntaTexto", "");
                nuevaPreguntaTipo = GetSession("NPS_nuevaPreguntaTipo", "");
                rangoMinimo = GetSession<int?>("NPS_rangoMinimo", null);
                rangoMaximo = GetSession<int?>("NPS_rangoMaximo", null);
                textoDetractor = GetSession("NPS_textoDetractor", "");
                textoNeutro = GetSession("NPS_textoNeutro", "");
                textoPromotor = GetSession("NPS_textoPromotor", "");
                textoValorMinimo = GetSession("NPS_textoValorMinimo", "");
                textoValorMaximo = GetSession("NPS_textoValorMaximo", "");
                nuevaAfirmacion = GetSession("NPS_nuevaAfirmacion", "");
                nuevaRespuestaLikert = GetSession("NPS_nuevaRespuestaLikert", "");
                respuestaPreguntaSimple = GetSession("NPS_respuestaPreguntaSimple", "");

                return Page();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException && ex.Message == "NO_TOKEN")
                    return Redirect("/");
                throw;
            }
        }

        // URL paginación (preserva filtros)
        public string PageUrl(int page)
        {
            page = Math.Max(1, page);
            return $"/Encuestas/NPS?currentPage={page}" +
                   $"&SearchTerm={Uri.EscapeDataString(SearchTerm ?? "")}" +
                   $"&SelectedEstado={Uri.EscapeDataString(SelectedEstado ?? "")}" +
                   $"&SelectedTipoPersona={Uri.EscapeDataString(SelectedTipoPersona ?? "")}" +
                   $"&FechaInicioFiltro={FechaInicioFiltro?.ToString("yyyy-MM-dd")}" +
                   $"&FechaFinFiltro={FechaFinFiltro?.ToString("yyyy-MM-dd")}";
        }

        // ========== lógica original: ApplyFilters ==========
        private void ApplyFilters()
        {
            var filtered = encuestas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                filtered = filtered.Where(e =>
                    e.NombreEncuesta != null &&
                    e.NombreEncuesta.ToLower().Contains(SearchTerm.ToLower()));

            if (!string.IsNullOrWhiteSpace(SelectedTipoPersona))
                filtered = filtered.Where(e =>
                    e.TipoPersona != null &&
                    e.TipoPersona.Equals(SelectedTipoPersona, StringComparison.OrdinalIgnoreCase));

            if (FechaInicioFiltro.HasValue)
                filtered = filtered.Where(e => e.FechaInicio >= FechaInicioFiltro.Value);

            if (FechaFinFiltro.HasValue)
                filtered = filtered.Where(e => e.FechaFin <= FechaFinFiltro.Value);

            if (!string.IsNullOrWhiteSpace(SelectedEstado))
                filtered = filtered.Where(e =>
                    e.Estado != null &&
                    e.Estado.Equals(SelectedEstado, StringComparison.OrdinalIgnoreCase));

            filtered = filtered.OrderBy(e => e.FechaInicio);

            var totalItems = filtered.Count();
            totalPages = Math.Max(1, (int)Math.Ceiling((double)totalItems / pageSize));
            currentPage = Math.Clamp(currentPage, 1, totalPages);

            pagedEncuestas = filtered
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        // ========== helpers originales ==========
        private async Task CargarPreguntas(EncuestaResponseDTO encuesta)
        {
            preguntasDisponibles = encuesta.EncuestaPreguntas
                .Select(p => new PreguntaResponse
                {
                    IdEncuesta = p.IdEncuesta,
                    TempId = p.IdEncuestaPregunta,
                    IdPregunta = p.IdEncuestaPregunta,
                    Texto = p.Pregunta,
                    Tipo = p.TipoPregunta,
                    RangoMin = p.RangoMinimo,
                    RangoMax = p.RangoMaximo,
                    FlagComentario = p.FlagComentario,
                    TextoDetractor = p.TextoDetractor ?? string.Empty,
                    TextoNeutro = p.TextoNeutro ?? string.Empty,
                    TextoValorMinimo = p.TextoValorMinimo ?? string.Empty,
                    TextoValorMaximo = p.TextoValorMaximo ?? string.Empty,
                    TextoPromotor = p.TextoPromotor ?? string.Empty,
                    Afirmaciones = string.IsNullOrWhiteSpace(p.ValoresX) ? new List<string>() : p.ValoresX.Split('|').ToList(),
                    RespuestasLikert = string.IsNullOrWhiteSpace(p.ValoresY) ? new List<string>() : p.ValoresY.Split('|').ToList()
                })
                .ToList();

            await Task.CompletedTask;
        }

        private EncuestaResponseDTO CopiarEncuesta(EncuestaResponseDTO encuesta) => new()
        {
            IdEncuesta = encuesta.IdEncuesta,
            NombreEncuesta = encuesta.NombreEncuesta,
            TipoPersona = encuesta.TipoPersona,
            FechaInicio = encuesta.FechaInicio,
            FechaFin = encuesta.FechaFin,
            FlagLogin = encuesta.FlagLogin,
            FlagBase = encuesta.FlagBase,
            FlagAnalisis = encuesta.FlagAnalisis,
            ImagenLogin = encuesta.ImagenLogin,
            TituloEncuesta = encuesta.TituloEncuesta,
            Link = encuesta.Link
        };

        // ========== POST handlers (equivalentes a @onclick) ==========
        public IActionResult OnPostCerrarModal()
        {
            SetSession("NPS_mostrarModal", false);
            return RedirectToSameListState();
        }

        public IActionResult OnPostCerrarModalPregunta()
        {
            SetSession("NPS_mostrarModalPregunta", false);
            SetSession<int?>("NPS_IdPreguntaEditando", null);
            return RedirectToSameListState();
        }

        public IActionResult OnPostCerrarEstadisticas()
        {
            SetSession("NPS_mostrarModalEstadisticas", false);
            return RedirectToSameListState();
        }

        public IActionResult OnPostCerrarQR()
        {
            SetSession("NPS_mostrarQRModal", false);
            SetSession("NPS_qrUrlGenerado", "");
            return RedirectToSameListState();
        }

        public IActionResult OnPostCerrarVistaPrevia()
        {
            SetSession("NPS_mostrarModalVistaPrevia", false);
            return RedirectToSameListState();
        }

        public IActionResult OnPostNuevaEncuesta()
        {
            ClearNpsSessionState();
            RequireTokenOrRedirect();
            IdEncuestaSeleccionada = 0;
            SetSession("NPS_IdEncuestaSeleccionada", 0);
            EncuestaConRespuestas = false;
            modoModal = "nuevo";
            formModel = new EncuestaResponseDTO();
            preguntasSeleccionadas.Clear();
            preguntasDisponibles.Clear();

            SetSession("NPS_EncuestaConRespuestas", EncuestaConRespuestas);
            SetSession("NPS_modoModal", modoModal);
            PersistDraftFormModel();
            SetSession("NPS_preguntasDisponibles", preguntasDisponibles);
            SetSession("NPS_esEdicion", false);
            SetSession("NPS_mostrarModal", true);

            return RedirectToSameListState();
        }

        public async Task<IActionResult> OnPostEditarEncuesta(int IdEncuesta)
        {
            RequireTokenOrRedirect();
            ClearNpsSessionState();
            // Encuesta desde lista (re-buscar para conservar igual)
            var lista = await ServiceRepositorio.ObtenerEncuestas(token!, "");
            var encuesta = lista.First(x => x.IdEncuesta == IdEncuesta);

            IdEncuestaSeleccionada = encuesta.IdEncuesta;

            EncuestaConRespuestas = encuesta.CantidadRespuestas > 0;
            modoModal = "editar";
            formModel = CopiarEncuesta(encuesta);

            await CargarPreguntas(encuesta);

            SetSession("NPS_IdEncuestaSeleccionada", IdEncuestaSeleccionada);
            SetSession("NPS_EncuestaConRespuestas", EncuestaConRespuestas);
            SetSession("NPS_modoModal", modoModal);
            SetSession("NPS_formModel", formModel);
            SetSession("NPS_preguntasDisponibles", preguntasDisponibles);
            SetSession("NPS_esEdicion", true);
            SetSession("NPS_mostrarModal", true);

            return RedirectToSameListState();
        }

        public async Task<IActionResult> OnPostVerDetalles(int IdEncuesta)
        {
            RequireTokenOrRedirect();
            HttpContext.Session.Remove("NPS_clientesEncuesta");
            var lista = await ServiceRepositorio.ObtenerEncuestas(token!, "");
            var encuesta = lista.First(x => x.IdEncuesta == IdEncuesta);
            ClearNpsSessionState();
            IdEncuestaSeleccionada = encuesta.IdEncuesta;
            modoModal = "ver";
            formModel = CopiarEncuesta(encuesta);

            await CargarPreguntas(encuesta);

            SetSession("NPS_IdEncuestaSeleccionada", IdEncuestaSeleccionada);
            SetSession("NPS_EncuestaConRespuestas", EncuestaConRespuestas);
            SetSession("NPS_modoModal", modoModal);
            PersistDraftFormModel();
            SetSession("NPS_preguntasDisponibles", preguntasDisponibles);
            SetSession("NPS_esEdicion", true);
            SetSession("NPS_mostrarModal", true);

            return RedirectToSameListState();
        }

        public IActionResult OnPostAbrirVistaPrevia()
        {
            RequireTokenOrRedirect();
            MergePostedFormModelWithSession();
            SetSession(SessFormModel, formModel);
            PersistDraftFormModel();

            esEdicion = GetSession("NPS_esEdicion", false);
            IdEncuestaSeleccionada = esEdicion ? formModel.IdEncuesta : 0;

            SetSession("NPS_IdEncuestaSeleccionada", IdEncuestaSeleccionada);
            SetSession("NPS_mostrarModalVistaPrevia", true);
            return RedirectToSameListState();
        }

        public IActionResult OnPostAbrirModalPregunta()
        {
            RequireTokenOrRedirect();

            // ✅ Guarda lo que ya se escribió en el modal de Encuesta (para que no se borre)
            PersistDraftFormModel();

            // reset como en Blazor
            esEdicionPregunta = false;
            preguntaEnEdicion = null;
            nuevaPreguntaTexto = "";
            nuevaPreguntaTipo = "";
            rangoMinimo = null;
            rangoMaximo = null;
            textoDetractor = "";
            textoNeutro = "";
            textoPromotor = "";
            textoValorMinimo = "";
            textoValorMaximo = "";
            afirmaciones = new();
            respuestasLikert = new();
            respuestaPreguntaSimple = "";
            IdPreguntaEditando = null;

            SetSession("NPS_esEdicionPregunta", esEdicionPregunta);
            SetSession("NPS_nuevaPreguntaTexto", nuevaPreguntaTexto);
            SetSession("NPS_nuevaPreguntaTipo", nuevaPreguntaTipo);
            SetSession<int?>("NPS_rangoMinimo", rangoMinimo);
            SetSession<int?>("NPS_rangoMaximo", rangoMaximo);
            SetSession("NPS_textoDetractor", textoDetractor);
            SetSession("NPS_textoNeutro", textoNeutro);
            SetSession("NPS_textoPromotor", textoPromotor);
            SetSession("NPS_textoValorMinimo", textoValorMinimo);
            SetSession("NPS_textoValorMaximo", textoValorMaximo);
            SetSession("NPS_afirmaciones", afirmaciones);
            SetSession("NPS_respuestasLikert", respuestasLikert);
            SetSession("NPS_respuestaPreguntaSimple", respuestaPreguntaSimple);
            SetSession<int?>("NPS_IdPreguntaEditando", IdPreguntaEditando);

            SetSession("NPS_mostrarModalPregunta", true);
            SetSession("NPS_mostrarModal", true); // mantener modal padre

            return RedirectToSameListState();
        }
        public async Task<IActionResult> OnPostCargarBase()
        {
            RequireTokenOrRedirect();

            SetSession("NPS_mostrarModal", true);

            PersistDraftFormModel();

            if (!formModel.FlagBase)
            {
                clientesEncuesta = new List<ClienteEncuestaDto>();
                SetSession("NPS_clientesEncuesta", clientesEncuesta);
                return RedirectToSameListState();
            }

            if (BaseFile == null || BaseFile.Length == 0)
            {
                clientesEncuesta = GetSession("NPS_clientesEncuesta", new List<ClienteEncuestaDto>());
                SetSession("NPS_clientesEncuesta", clientesEncuesta);
                return RedirectToSameListState();
            }

            var parsed = await ParseBaseFileAsync(BaseFile);
            if (parsed.Any())
            {
                clientesEncuesta = parsed;
                SetSession("NPS_clientesEncuesta", clientesEncuesta);
            }
            else
            {
                clientesEncuesta = new List<ClienteEncuestaDto>();
                SetSession("NPS_clientesEncuesta", clientesEncuesta);
            }

            return RedirectToSameListState();
        }
        public IActionResult OnPostCambioTipoPregunta(string nuevaPreguntaTipo)
        {
            RequireTokenOrRedirect();

            PersistDraftFormModel();

            SetSession("NPS_nuevaPreguntaTexto", nuevaPreguntaTexto ?? "");
            SetSession("NPS_nuevaPreguntaTipo", nuevaPreguntaTipo ?? "");

            SetSession("NPS_mostrarModalPregunta", true);
            SetSession("NPS_mostrarModal", true);
            return RedirectToSameListState();
        }

        public IActionResult OnPostAgregarAfirmacion()
        {
            PersistDraftFormModel();
            SetSession(SessFormModel, formModel);
            SetSession("NPS_nuevaPreguntaTexto", nuevaPreguntaTexto ?? "");
            afirmaciones = GetSession("NPS_afirmaciones", new List<string>());
            if (!string.IsNullOrWhiteSpace(nuevaAfirmacion))
                afirmaciones.Add(nuevaAfirmacion);

            SetSession("NPS_afirmaciones", afirmaciones);
            SetSession("NPS_mostrarModalPregunta", true);
            SetSession("NPS_mostrarModal", true);
            return RedirectToSameListState();
        }

        public IActionResult OnPostEliminarAfirmacion(string valor)
        {
            PersistDraftFormModel();
            SetSession(SessFormModel, formModel);
            afirmaciones = GetSession("NPS_afirmaciones", new List<string>());
            afirmaciones.Remove(valor);
            SetSession("NPS_afirmaciones", afirmaciones);
            SetSession("NPS_mostrarModalPregunta", true);
            SetSession("NPS_mostrarModal", true);
            return RedirectToSameListState();
        }

        public IActionResult OnPostAgregarRespuestaLikert()
        {
            PersistDraftFormModel();
            SetSession(SessFormModel, formModel);
            SetSession("NPS_nuevaPreguntaTexto", nuevaPreguntaTexto ?? "");
            respuestasLikert = GetSession("NPS_respuestasLikert", new List<string>());
            if (!string.IsNullOrWhiteSpace(nuevaRespuestaLikert))
                respuestasLikert.Add(nuevaRespuestaLikert);

            SetSession("NPS_respuestasLikert", respuestasLikert);
            SetSession("NPS_mostrarModalPregunta", true);
            SetSession("NPS_mostrarModal", true);
            return RedirectToSameListState();
        }

        public IActionResult OnPostEliminarRespuestaLikert(string valor)
        {
            PersistDraftFormModel();
            SetSession(SessFormModel, formModel);
            respuestasLikert = GetSession("NPS_respuestasLikert", new List<string>());
            respuestasLikert.Remove(valor);
            SetSession("NPS_respuestasLikert", respuestasLikert);
            SetSession("NPS_mostrarModalPregunta", true);
            SetSession("NPS_mostrarModal", true);
            return RedirectToSameListState();
        }

        public IActionResult OnPostEditarPregunta(int TempId)
        {
            RequireTokenOrRedirect();
            PersistDraftFormModel();
            SetSession(SessFormModel, formModel);
            SetSession("NPS_mostrarModal", true);
            preguntasDisponibles = GetSession("NPS_preguntasDisponibles", new List<PreguntaResponse>());
            var pregunta = preguntasDisponibles.FirstOrDefault(p => p.TempId == TempId);
            if (pregunta == null) return RedirectToPage();

            esEdicionPregunta = true;
            IdPreguntaEditando = pregunta.TempId;
            nuevaPreguntaTexto = pregunta.Texto;
            nuevaPreguntaTipo = pregunta.Tipo;
            rangoMinimo = pregunta.RangoMin;
            rangoMaximo = pregunta.RangoMax;
            textoDetractor = pregunta.TextoDetractor ?? "";
            textoNeutro = pregunta.TextoNeutro ?? "";
            textoPromotor = pregunta.TextoPromotor ?? "";
            textoValorMinimo = pregunta.TextoValorMinimo ?? "";
            textoValorMaximo = pregunta.TextoValorMaximo ?? "";
            afirmaciones = pregunta.Afirmaciones?.ToList() ?? new List<string>();
            respuestasLikert = pregunta.RespuestasLikert?.ToList() ?? new List<string>();
            respuestaPreguntaSimple = pregunta.RespuestaSimple ?? "";

            SetSession("NPS_esEdicionPregunta", esEdicionPregunta);
            SetSession<int?>("NPS_IdPreguntaEditando", IdPreguntaEditando);
            SetSession("NPS_nuevaPreguntaTexto", nuevaPreguntaTexto);
            SetSession("NPS_nuevaPreguntaTipo", nuevaPreguntaTipo);
            SetSession<int?>("NPS_rangoMinimo", rangoMinimo);
            SetSession<int?>("NPS_rangoMaximo", rangoMaximo);
            SetSession("NPS_textoDetractor", textoDetractor);
            SetSession("NPS_textoNeutro", textoNeutro);
            SetSession("NPS_textoPromotor", textoPromotor);
            SetSession("NPS_textoValorMinimo", textoValorMinimo);
            SetSession("NPS_textoValorMaximo", textoValorMaximo);
            SetSession("NPS_afirmaciones", afirmaciones);
            SetSession("NPS_respuestasLikert", respuestasLikert);
            SetSession("NPS_respuestaPreguntaSimple", respuestaPreguntaSimple);

            SetSession("NPS_mostrarModalPregunta", true);
            SetSession("NPS_mostrarModal", true);
            return RedirectToSameListState();
        }

        public IActionResult OnPostEliminarPregunta(int TempId)
        {
            PersistDraftFormModel();
            SetSession(SessFormModel, formModel);
            RequireTokenOrRedirect();
            SetSession("NPS_mostrarModal", true);
            preguntasDisponibles = GetSession("NPS_preguntasDisponibles", new List<PreguntaResponse>());
            preguntasEliminadas = GetSession("NPS_preguntasEliminadas", new List<PreguntasEliminar>());

            var pregunta = preguntasDisponibles.FirstOrDefault(p => p.TempId == TempId);
            if (pregunta == null) return RedirectToPage();

            if (pregunta.IdPregunta > 0)
                preguntasEliminadas.Add(new PreguntasEliminar { IdPregunta = pregunta.IdPregunta });

            preguntasDisponibles.Remove(pregunta);

            SetSession("NPS_preguntasDisponibles", preguntasDisponibles);
            SetSession("NPS_preguntasEliminadas", preguntasEliminadas);
            SetSession("NPS_mostrarModal", true);

            return RedirectToSameListState();
        }

        public IActionResult OnPostGuardarPregunta()
        {
            PersistDraftFormModel();
            SetSession(SessFormModel, formModel);
            RequireTokenOrRedirect();

            // Rehidratar
            preguntasDisponibles = GetSession("NPS_preguntasDisponibles", new List<PreguntaResponse>());
            afirmaciones = GetSession("NPS_afirmaciones", new List<string>());
            respuestasLikert = GetSession("NPS_respuestasLikert", new List<string>());
            IdPreguntaEditando = GetSession<int?>("NPS_IdPreguntaEditando", null);

            if (string.IsNullOrWhiteSpace(nuevaPreguntaTexto) || string.IsNullOrWhiteSpace(nuevaPreguntaTipo))
                return RedirectToPage();

            if (nuevaPreguntaTipo == "Escala Numérica" && (!rangoMinimo.HasValue || !rangoMaximo.HasValue))
                return RedirectToPage();

            if (nuevaPreguntaTipo == "Escala de Likert" && (afirmaciones.Count == 0 || respuestasLikert.Count == 0))
                return RedirectToPage();

            var flagComentario = !string.IsNullOrWhiteSpace(textoNeutro)
                              || !string.IsNullOrWhiteSpace(textoDetractor)
                              || !string.IsNullOrWhiteSpace(textoPromotor);

            if (IdPreguntaEditando.HasValue)
            {
                var pregunta = preguntasDisponibles.FirstOrDefault(p => p.TempId == IdPreguntaEditando.Value);
                if (pregunta != null)
                {
                    pregunta.Texto = nuevaPreguntaTexto;
                    pregunta.Tipo = nuevaPreguntaTipo;
                    pregunta.RangoMin = rangoMinimo;
                    pregunta.RangoMax = rangoMaximo;
                    pregunta.TextoDetractor = textoDetractor;
                    pregunta.TextoNeutro = textoNeutro;
                    pregunta.TextoPromotor = textoPromotor;
                    pregunta.TextoValorMinimo = textoValorMinimo;
                    pregunta.TextoValorMaximo = textoValorMaximo;
                    pregunta.Afirmaciones = afirmaciones.ToList();
                    pregunta.RespuestasLikert = respuestasLikert.ToList();
                    pregunta.RespuestaSimple = respuestaPreguntaSimple;
                    pregunta.FlagComentario = flagComentario;
                }
            }
            else
            {
                var nextTempId = preguntasDisponibles.Any()
                                    ? Math.Min(preguntasDisponibles.Min(x => x.TempId), 0) - 1
                                    : -1;
                preguntasDisponibles.Add(new PreguntaResponse
                {
                    TempId = nextTempId,
                    IdPregunta = 0,
                    Texto = nuevaPreguntaTexto,
                    Tipo = nuevaPreguntaTipo,
                    RangoMin = rangoMinimo,
                    RangoMax = rangoMaximo,
                    TextoDetractor = textoDetractor,
                    TextoNeutro = textoNeutro,
                    TextoPromotor = textoPromotor,
                    TextoValorMinimo = textoValorMinimo,
                    TextoValorMaximo = textoValorMaximo,
                    Afirmaciones = afirmaciones.ToList(),
                    RespuestasLikert = respuestasLikert.ToList(),
                    RespuestaSimple = respuestaPreguntaSimple,
                    FlagComentario = flagComentario
                });
            }

            // Reset
            SetSession<int?>("NPS_IdPreguntaEditando", null);
            SetSession("NPS_preguntasDisponibles", preguntasDisponibles);
            SetSession("NPS_mostrarModalPregunta", false);
            SetSession("NPS_mostrarModal", true);

            return RedirectToSameListState();
        }

        public async Task<IActionResult> OnPostGuardarEncuesta()
        {
            try
            {
                RequireTokenOrRedirect();

                // rehidratar estado complejo
                var sessionModel = GetSession("NPS_formModel", new EncuestaResponseDTO());

                // Merge seguro: solo rellena vacíos, pero NO pises lo que vino del POST
                if (string.IsNullOrWhiteSpace(formModel?.NombreEncuesta))
                    formModel.NombreEncuesta = sessionModel.NombreEncuesta;

                if (string.IsNullOrWhiteSpace(formModel?.TituloEncuesta))
                    formModel.TituloEncuesta = sessionModel.TituloEncuesta;

                if (string.IsNullOrWhiteSpace(formModel?.TipoPersona))
                    formModel.TipoPersona = sessionModel.TipoPersona;

                if (formModel?.FechaInicio == null || formModel.FechaInicio == default)
                    formModel.FechaInicio = sessionModel.FechaInicio;

                if (formModel?.FechaFin == null || formModel.FechaFin == default)
                    formModel.FechaFin = sessionModel.FechaFin;
                if ((ImagenLoginFile == null || ImagenLoginFile.Length == 0) && sessionModel.ImagenLogin != null && sessionModel.ImagenLogin.Length > 0)
                {
                    formModel.ImagenLogin = sessionModel.ImagenLogin;
                }
                if (formModel.FlagBase)
                {
                    if (BaseFile != null && BaseFile.Length > 0)
                    {
                        var parsed = await ParseBaseFileAsync(BaseFile);
                        if (parsed.Any())
                        {
                            clientesEncuesta = parsed;
                            SetSession("NPS_clientesEncuesta", clientesEncuesta);
                        }
                        else
                        {
                            clientesEncuesta = GetSession("NPS_clientesEncuesta", new List<ClienteEncuestaDto>());
                        }
                    }
                    else
                    {
                        clientesEncuesta = GetSession("NPS_clientesEncuesta", new List<ClienteEncuestaDto>());
                    }
                }
                else
                {
                    clientesEncuesta = new List<ClienteEncuestaDto>();
                    SetSession("NPS_clientesEncuesta", clientesEncuesta);
                }
                if (formModel.IdEncuesta <= 0 && sessionModel.IdEncuesta > 0)
                    formModel.IdEncuesta = sessionModel.IdEncuesta;
                preguntasDisponibles = GetSession("NPS_preguntasDisponibles", new List<PreguntaResponse>());
                preguntasEliminadas = GetSession("NPS_preguntasEliminadas", new List<PreguntasEliminar>());
                clientesEncuesta = GetSession("NPS_clientesEncuesta", new List<ClienteEncuestaDto>());
                esEdicion = GetSession("NPS_esEdicion", false);

                if (ImagenLoginFile != null && ImagenLoginFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await ImagenLoginFile.CopyToAsync(ms);
                    formModel.ImagenLogin = ms.ToArray();
                }

                if (esEdicion)
                {
                    List<ActualizaEncuestaPreguntaDTO> listadoPreguntas = new();

                    for (int i = 0; i < preguntasDisponibles.Count; i++)
                    {
                        var p = preguntasDisponibles[i];

                        string valoresX = string.Join("|", p.Afirmaciones ?? new List<string>());
                        string valoresY = string.Join("|", p.RespuestasLikert ?? new List<string>());

                        listadoPreguntas.Add(new ActualizaEncuestaPreguntaDTO
                        {
                            IdEncuestaPregunta = p.IdPregunta,
                            RangoMinimo = p.RangoMin,
                            RangoMaximo = p.RangoMax,
                            Orden = i + 1,
                            ValoresX = valoresX,
                            ValoresY = valoresY,
                            Pregunta = p.Texto,
                            TipoPregunta = p.Tipo,
                            FlagComentario = p.FlagComentario,
                            TextoDetractor = p.TextoDetractor,
                            TextoNeutro = p.TextoNeutro,
                            TextoPromotor = p.TextoPromotor,
                            TextoValorMinimo = p.TextoValorMinimo,
                            TextoValorMaximo = p.TextoValorMaximo
                        });
                    }

                    var request = new ModificarEncuestaDTO
                    {
                        Usuario = usuarioLogeado,
                        DatosEncuesta = new ActualizaEncuestaDTO
                        {
                            IdEncuesta = formModel.IdEncuesta,
                            FechaInicio = DateTime.Parse(formModel.FechaInicio.ToString()!),
                            FechaFin = DateTime.Parse(formModel.FechaFin.ToString()!),
                            FlagLogin = formModel.FlagLogin,
                            FlagBase = formModel.FlagBase,
                            ImagenLogin = formModel.ImagenLogin,
                            TituloEncuesta = formModel.TituloEncuesta,
                            NombreEncuesta = formModel.NombreEncuesta,

                        },
                        EncuestaPreguntas = listadoPreguntas,
                        NPS_ClienteEncuesta = clientesEncuesta,
                        preguntasEliminadas = preguntasEliminadas
                    };

                    var result = await ServiceRepositorio.ActualizarEncuesta(token!, request);

                    if (result != null)
                    {
                        modalTitle = "✅ Éxito";
                        modalMessage = "La encuesta fue creada correctamente.";
                        PersistDraftFormModel();
                    }
                    else
                    {
                        modalTitle = "❌ Error";
                        modalMessage = "Ocurrió un problema al crear la encuesta.";
                    }
                }
                else
                {
                    List<EncuestaPreguntaDTO> listadoPreguntas = new();

                    for (int i = 0; i < preguntasDisponibles.Count; i++)
                    {
                        var p = preguntasDisponibles[i];

                        string valoresX = string.Join("|", p.Afirmaciones ?? new List<string>());
                        string valoresY = string.Join("|", p.RespuestasLikert ?? new List<string>());

                        listadoPreguntas.Add(new EncuestaPreguntaDTO
                        {
                            RangoMinimo = p.RangoMin,
                            RangoMaximo = p.RangoMax,
                            Orden = i + 1,
                            ValoresX = valoresX,
                            ValoresY = valoresY,
                            Pregunta = p.Texto,
                            TipoPregunta = p.Tipo,
                            FlagComentario = p.FlagComentario,
                            TextoDetractor = p.TextoDetractor,
                            TextoNeutro = p.TextoNeutro,
                            TextoPromotor = p.TextoPromotor,
                            TextoValorMinimo = p.TextoValorMinimo,
                            TextoValorMaximo = p.TextoValorMaximo
                        });
                    }

                    var request = new CrearEncuestaDTO
                    {
                        Usuario = usuarioLogeado,
                        DatosEncuesta = new EncuestaDTO
                        {
                            NombreEncuesta = formModel.NombreEncuesta,
                            TipoPersona = formModel.TipoPersona,
                            FechaInicio = DateTime.Parse(formModel.FechaInicio.ToString()!),
                            FechaFin = DateTime.Parse(formModel.FechaFin.ToString()!),
                            TituloEncuesta = formModel.TituloEncuesta,
                            FlagLogin = formModel.FlagLogin,
                            FlagBase = formModel.FlagBase,
                            FlagAnalisis = true,
                            ImagenLogin = formModel.ImagenLogin
                        },
                        EncuestaPreguntas = listadoPreguntas,
                        NPS_ClienteEncuesta = clientesEncuesta
                    };

                    var result = await ServiceRepositorio.CrearEncuesta(token!, request);

                    if (result != null)
                    {
                        modalTitle = "✅ Éxito";
                        modalMessage = "La encuesta fue creada correctamente.";
                        PersistDraftFormModel();
                    }
                    else
                    {
                        modalTitle = "❌ Error";
                        modalMessage = "Ocurrió un problema al crear la encuesta.";
                    }
                }

                // Recargar
                encuestas = await ServiceRepositorio.ObtenerEncuestas(token!, "");
                ApplyFilters();

                SetSession("NPS_mostrarModal", false);
                return RedirectToSameListState();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException && ex.Message == "NO_TOKEN")
                    return Redirect("/");

                throw;
            }
        }

        public IActionResult OnPostAbrirEstadisticas()
        {
            RequireTokenOrRedirect();
            SetSession("NPS_mostrarModalEstadisticas", true);
            SetSession("NPS_mostrarModal", true);
            return RedirectToSameListState();
        }

        public async Task<IActionResult> OnPostDescargarBase(int IdEncuesta)
        {
            try
            {
                RequireTokenOrRedirect();

                isLoading = true;

                BaseclientesEncuesta = await ServiceRepositorio.ObtenerBaseCLientesEncuestas(token!, IdEncuesta);
                var lista = await ServiceRepositorio.ObtenerEncuestas(token!, "");
                var encuesta = lista.FirstOrDefault(x => x.IdEncuesta == IdEncuesta);
                if (encuesta == null)
                    return RedirectToPage();
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Base");

                worksheet.Cells["A1"].Value = "NroDocumento";
                worksheet.Cells["B1"].Value = "Codigo IBS";
                worksheet.Cells["C1"].Value = "Nombre";
                worksheet.Cells["D1"].Value = "Correo";
                worksheet.Cells["E1"].Value = "Celular";
                worksheet.Cells["F1"].Value = "Link";
                worksheet.Cells["G1"].Value = "Contestó";

                using (var range = worksheet.Cells["A1:G1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int row = 2;
                foreach (var item in BaseclientesEncuesta)
                {
                    worksheet.Cells[row, 1].Value = item.NroDocumento;
                    worksheet.Cells[row, 2].Value = item.CodigoIBS;
                    worksheet.Cells[row, 3].Value = item.Nombre;
                    worksheet.Cells[row, 4].Value = item.Correo;
                    worksheet.Cells[row, 5].Value = item.Celular;
                    worksheet.Cells[row, 6].Value = item.LinkPersonalizado;
                    worksheet.Cells[row, 7].Value = item.FlagContesta == 1 ? "Sí" : "No";
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var bytes = package.GetAsByteArray();

                // 🔹 limpiar nombre de encuesta para nombre de archivo
                var safeName = string.Join("_",
                    encuesta.NombreEncuesta.Split(Path.GetInvalidFileNameChars(),
                    StringSplitOptions.RemoveEmptyEntries)).Trim();

                var fechaHoy = DateTime.Now.ToString("yyyyMMdd");

                var fileName = $"BaseClientes_{safeName}_{fechaHoy}.xlsx";

                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException && ex.Message == "NO_TOKEN")
                    return Redirect("/");

                throw;
            }
        }

        public async Task<IActionResult> OnPostDescargarRespuestas(int IdEncuesta)
        {
            try
            {
                RequireTokenOrRedirect();

                isLoading = true;

                // Obtener encuesta para nombre (igual que Blazor usaba encuesta.NombreEncuesta)
                var lista = await ServiceRepositorio.ObtenerEncuestas(token!, "");
                var encuesta = lista.First(x => x.IdEncuesta == IdEncuesta);

                var export = await ServiceRepositorio.ExportarRespuestasEncuesta(token!, IdEncuesta);

                if (export is null || export.Columns.Count == 0 || export.Rows is null || export.Rows.Count == 0)
                {
                    // si quieres, puedes mandar TempData para mostrar alert
                    return RedirectToPage();
                }
                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Respuestas");

                int totalCols = export.Columns.Count;

                for (int c = 0; c < totalCols; c++)
                    ws.Cells[1, c + 1].Value = export.Columns[c].Header;

                using (var range = ws.Cells[1, 1, 1, totalCols])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.WrapText = false;
                }

                ws.Row(1).Height = 18;

                for (int r = 0; r < export.Rows.Count; r++)
                {
                    var rowDict = export.Rows[r];

                    for (int c = 0; c < totalCols; c++)
                    {
                        var key = export.Columns[c].Key;
                        rowDict.TryGetValue(key, out var value);

                        ws.Cells[r + 2, c + 1].Value = value ?? "";
                        ws.Cells[r + 2, c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[r + 2, c + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[r + 2, c + 1].Style.WrapText = true;
                    }
                }

                ws.View.FreezePanes(2, 1);

                for (int c = 1; c <= totalCols; c++)
                    ws.Column(c).Width = 20;

                if (totalCols >= 1) ws.Column(1).Width = 18;
                if (totalCols >= 2) ws.Column(2).Width = 18;
                if (totalCols >= 3) ws.Column(3).Width = 20;
                if (totalCols >= 4) ws.Column(4).Width = 26;

                var bytes = package.GetAsByteArray();
                var nombre = $"Respuestas_{encuesta.NombreEncuesta}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombre);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException && ex.Message == "NO_TOKEN")
                    return Redirect("/");

                throw;
            }
        }

        public async Task<IActionResult> OnPostGenerarQR(int IdEncuesta)
        {
            RequireTokenOrRedirect();

            var lista = await ServiceRepositorio.ObtenerEncuestas(token!, "");
            var encuesta = lista.First(x => x.IdEncuesta == IdEncuesta);

            qrUrlGenerado = $"{encuesta.Link}";
            SetSession("NPS_qrUrlGenerado", qrUrlGenerado);
            SetSession("NPS_mostrarQRModal", true);
            return RedirectToSameListState();
        }

        public async Task<IActionResult> OnPostDescargarQR()
        {
            RequireTokenOrRedirect();

            qrUrlGenerado = GetSession("NPS_qrUrlGenerado", "");
            if (string.IsNullOrWhiteSpace(qrUrlGenerado))
                return RedirectToSameListState();

            var qrImgUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=400x400&data={Uri.EscapeDataString(qrUrlGenerado)}";
            using var http = new HttpClient();
            var bytes = await http.GetByteArrayAsync(qrImgUrl);

            var nombreArchivo = $"QR_Encuesta_{DateTime.Now:yyyyMMddHHmmss}.png";
            return File(bytes, "image/png", nombreArchivo);
        }

        public IActionResult OnPostExportarExcel()
        {
            // Tu método en Blazor era placeholder.
            // Si quieres, aquí armamos el excel de la tabla encuestas filtrada.
            return RedirectToSameListState();
        }
        private IActionResult RedirectToSameListState()
        {
            return RedirectToPage(new
            {
                currentPage,
                SearchTerm,
                SelectedEstado,
                SelectedTipoPersona,
                FechaInicioFiltro = FechaInicioFiltro?.ToString("yyyy-MM-dd"),
                FechaFinFiltro = FechaFinFiltro?.ToString("yyyy-MM-dd")
            });
        }
        private static string Clean(string? s) => (s ?? "").Trim();

        private static string CleanPhone(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            return new string(s.Where(char.IsDigit).ToArray());
        }

        private async Task<List<ClienteEncuestaDto>> ParseBaseFileAsync(IFormFile file)
        {
            var list = new List<ClienteEncuestaDto>();
            if (file == null || file.Length == 0) return list;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            if (ext == ".xlsx")
                return ParseBaseExcel(ms);

            if (ext == ".csv")
                return ParseBaseCsv(ms);

            return list;
        }

        private List<ClienteEncuestaDto> ParseBaseExcel(Stream stream)
        {
            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.FirstOrDefault();
            if (ws?.Dimension == null) return new List<ClienteEncuestaDto>();

            int startRow = ws.Dimension.Start.Row;
            int endRow = ws.Dimension.End.Row;

            // Tu lógica antigua (Aspose) empezaba desde row=1 (0 era header).
            // EPPlus: startRow es normalmente 1, así que data empieza en 2.
            int dataStartRow = startRow + 1;

            var list = new List<ClienteEncuestaDto>();

            for (int r = dataStartRow; r <= endRow; r++)
            {
                var nroDoc = Clean(ws.Cells[r, 1].Text); // Col A
                var ibs = Clean(ws.Cells[r, 2].Text); // Col B
                var nombre = Clean(ws.Cells[r, 3].Text); // Col C
                var correo = Clean(ws.Cells[r, 4].Text); // Col D
                var celular = Clean(ws.Cells[r, 5].Text); // Col E

                // si fila vacía, saltar
                if (string.IsNullOrWhiteSpace(nroDoc) &&
                    string.IsNullOrWhiteSpace(ibs) &&
                    string.IsNullOrWhiteSpace(nombre) &&
                    string.IsNullOrWhiteSpace(correo) &&
                    string.IsNullOrWhiteSpace(celular))
                    continue;

                // mínimo documento (igual que en tu enfoque)
                if (string.IsNullOrWhiteSpace(nroDoc))
                    continue;

                list.Add(new ClienteEncuestaDto
                {
                    NroDocumento = nroDoc,
                    CodigoIBS = ibs,
                    Nombre = nombre,
                    Correo = correo,
                    Celular = CleanPhone(celular)
                });
            }

            // quitar duplicados por documento
            return list
                .GroupBy(x => x.NroDocumento)
                .Select(g => g.First())
                .ToList();
        }

        private List<ClienteEncuestaDto> ParseBaseCsv(Stream stream)
        {
            using var sr = new StreamReader(stream);
            var lines = new List<string>();
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }
            if (lines.Count < 2) return new List<ClienteEncuestaDto>();

            char sep = lines[0].Contains(';') ? ';' : ',';

            // asume que la primera fila es header y los datos vienen desde la segunda,
            // igual que tu Excel
            var list = new List<ClienteEncuestaDto>();

            for (int i = 1; i < lines.Count; i++)
            {
                var cols = lines[i].Split(sep);
                string nroDoc = cols.Length > 0 ? Clean(cols[0]) : "";
                if (string.IsNullOrWhiteSpace(nroDoc)) continue;

                string ibs = cols.Length > 1 ? Clean(cols[1]) : "";
                string nombre = cols.Length > 2 ? Clean(cols[2]) : "";
                string correo = cols.Length > 3 ? Clean(cols[3]) : "";
                string celular = cols.Length > 4 ? Clean(cols[4]) : "";

                list.Add(new ClienteEncuestaDto
                {
                    NroDocumento = nroDoc,
                    CodigoIBS = ibs,
                    Nombre = nombre,
                    Correo = correo,
                    Celular = CleanPhone(celular)
                });
            }

            return list
                .GroupBy(x => x.NroDocumento)
                .Select(g => g.First())
                .ToList();
        }
        public IActionResult OnPostExportarBaseCargada()
        {
            RequireTokenOrRedirect();

            var baseCargada = GetSession("NPS_clientesEncuesta", new List<ClienteEncuestaDto>());
            if (baseCargada == null || baseCargada.Count == 0)
                return RedirectToSameListState();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Base");

            worksheet.Cells["A1"].Value = "NroDocumento";
            worksheet.Cells["B1"].Value = "Codigo IBS";
            worksheet.Cells["C1"].Value = "Nombre";
            worksheet.Cells["D1"].Value = "Correo";
            worksheet.Cells["E1"].Value = "Celular";

            using (var range = worksheet.Cells["A1:E1"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            int row = 2;
            foreach (var item in baseCargada)
            {
                worksheet.Cells[row, 1].Value = item.NroDocumento;
                worksheet.Cells[row, 2].Value = item.CodigoIBS;
                worksheet.Cells[row, 3].Value = item.Nombre;
                worksheet.Cells[row, 4].Value = item.Correo;
                worksheet.Cells[row, 5].Value = item.Celular;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var bytes = package.GetAsByteArray();
            var fileName = $"BaseCargada_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        public async Task<IActionResult> OnGetLoginImage(int idEncuesta)
        {
            // si no hay sesión/token, no se puede pedir al API
            token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized();

            // trae la encuesta y su imagen (ajusta si tienes un endpoint directo)
            var lista = await ServiceRepositorio.ObtenerEncuestas(token, "");
            var encuesta = lista.FirstOrDefault(x => x.IdEncuesta == idEncuesta);

            var bytes = encuesta?.ImagenLogin;
            if (bytes == null || bytes.Length == 0)
                return NotFound();

            // detectar content-type simple
            var contentType = GetImageContentType(bytes);
            return File(bytes, contentType);
        }

        private static string GetImageContentType(byte[] bytes)
        {
            // PNG
            if (bytes.Length >= 8 &&
                bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                return "image/png";

            // JPG
            if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8)
                return "image/jpeg";

            // GIF
            if (bytes.Length >= 3 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                return "image/gif";

            return "application/octet-stream";
        }
        public async Task<IActionResult> OnPostCargarImagenLogin()
        {
            RequireTokenOrRedirect();

            SetSession("NPS_mostrarModal", true);

            MergePostedFormModelWithSession();

            if (!formModel.FlagLogin)
            {
                HttpContext.Session.Remove(SessImgDraft);
                formModel.ImagenLogin = null;
                SetSession(SessFormModel, formModel);
                return RedirectToSameListState();
            }

            if (ImagenLoginFile == null || ImagenLoginFile.Length == 0)
            {
                SetSession(SessFormModel, formModel);
                return RedirectToSameListState();
            }

            using var ms = new MemoryStream();
            await ImagenLoginFile.CopyToAsync(ms);
            var bytes = ms.ToArray();

            SetSession<byte[]?>(SessImgDraft, bytes);

            formModel.ImagenLogin = bytes;
            SetSession(SessFormModel, formModel);

            return RedirectToSameListState();
        }

        public IActionResult OnGetLoginImageDraft()
        {
            var draft = GetSession<byte[]?>(SessImgDraft, null);
            if (draft != null && draft.Length > 0)
                return File(draft, GetImageContentType(draft));

            var model = GetSession(SessFormModel, new EncuestaResponseDTO());
            var bytes = model.ImagenLogin;

            if (bytes == null || bytes.Length == 0)
                return NotFound();

            return File(bytes, GetImageContentType(bytes));
        }
        private void MergePostedFormModelWithSession()
        {
            var sessionModel = GetSession(SessFormModel, new EncuestaResponseDTO());
            var draftBytes = GetSession<byte[]?>(SessImgDraft, null);

            // Si el post NO trae imagen, conserva: draft > sessionModel
            if (formModel.ImagenLogin == null || formModel.ImagenLogin.Length == 0)
            {
                if (draftBytes != null && draftBytes.Length > 0)
                    formModel.ImagenLogin = draftBytes;
                else if (sessionModel.ImagenLogin != null && sessionModel.ImagenLogin.Length > 0)
                    formModel.ImagenLogin = sessionModel.ImagenLogin;
            }

            // (Opcional) conserva otros campos si vienen vacíos
            if (string.IsNullOrWhiteSpace(formModel.NombreEncuesta)) formModel.NombreEncuesta = sessionModel.NombreEncuesta;
            if (string.IsNullOrWhiteSpace(formModel.TituloEncuesta)) formModel.TituloEncuesta = sessionModel.TituloEncuesta;
            if (string.IsNullOrWhiteSpace(formModel.TipoPersona)) formModel.TipoPersona = sessionModel.TipoPersona;
            if (formModel.FechaInicio == null || formModel.FechaInicio == default) formModel.FechaInicio = sessionModel.FechaInicio;
            if (formModel.FechaFin == null || formModel.FechaFin == default) formModel.FechaFin = sessionModel.FechaFin;
        }
        private void ClearNpsSessionState()
        {
            // Flags / modales
            HttpContext.Session.Remove("NPS_mostrarModal");
            HttpContext.Session.Remove("NPS_modoModal");
            HttpContext.Session.Remove("NPS_EncuestaConRespuestas");
            HttpContext.Session.Remove("NPS_esEdicion");

            HttpContext.Session.Remove("NPS_mostrarModalPregunta");
            HttpContext.Session.Remove("NPS_esEdicionPregunta");
            HttpContext.Session.Remove("NPS_IdPreguntaEditando");

            HttpContext.Session.Remove("NPS_mostrarModalEstadisticas");
            HttpContext.Session.Remove("NPS_mostrarQRModal");
            HttpContext.Session.Remove("NPS_qrUrlGenerado");
            HttpContext.Session.Remove("NPS_mostrarModalVistaPrevia");
            HttpContext.Session.Remove("NPS_IdEncuestaSeleccionada");

            // Estado de trabajo
            HttpContext.Session.Remove("NPS_formModel");
            HttpContext.Session.Remove("NPS_preguntasDisponibles");
            HttpContext.Session.Remove("NPS_preguntasEliminadas");

            // Base cargada
            HttpContext.Session.Remove("NPS_clientesEncuesta");

            // Campos modal pregunta
            HttpContext.Session.Remove("NPS_nuevaPreguntaTexto");
            HttpContext.Session.Remove("NPS_nuevaPreguntaTipo");
            HttpContext.Session.Remove("NPS_rangoMinimo");
            HttpContext.Session.Remove("NPS_rangoMaximo");
            HttpContext.Session.Remove("NPS_textoDetractor");
            HttpContext.Session.Remove("NPS_textoNeutro");
            HttpContext.Session.Remove("NPS_textoPromotor");
            HttpContext.Session.Remove("NPS_textoValorMinimo");
            HttpContext.Session.Remove("NPS_textoValorMaximo");
            HttpContext.Session.Remove("NPS_afirmaciones");
            HttpContext.Session.Remove("NPS_respuestasLikert");
            HttpContext.Session.Remove("NPS_nuevaAfirmacion");
            HttpContext.Session.Remove("NPS_nuevaRespuestaLikert");
            HttpContext.Session.Remove("NPS_respuestaPreguntaSimple");

            // Imagen draft (si implementaste ese approach)
            HttpContext.Session.Remove("NPS_ImagenLoginDraft");
        }
        private EncuestaResponseDTO MergeFormModelKeepingBinary()
        {
            var sessionModel = GetSession("NPS_formModel", new EncuestaResponseDTO());

            // --- Merge campos "normales" (lo que venga del POST manda si no es vacío)
            sessionModel.IdEncuesta = (formModel.IdEncuesta > 0) ? formModel.IdEncuesta : sessionModel.IdEncuesta;

            sessionModel.NombreEncuesta = !string.IsNullOrWhiteSpace(formModel.NombreEncuesta)
                ? formModel.NombreEncuesta
                : sessionModel.NombreEncuesta;

            sessionModel.TituloEncuesta = !string.IsNullOrWhiteSpace(formModel.TituloEncuesta)
                ? formModel.TituloEncuesta
                : sessionModel.TituloEncuesta;

            sessionModel.TipoPersona = !string.IsNullOrWhiteSpace(formModel.TipoPersona)
                ? formModel.TipoPersona
                : sessionModel.TipoPersona;

            sessionModel.FechaInicio = (formModel.FechaInicio.HasValue && formModel.FechaInicio.Value != default)
                ? formModel.FechaInicio
                : sessionModel.FechaInicio;

            sessionModel.FechaFin = (formModel.FechaFin.HasValue && formModel.FechaFin.Value != default)
                ? formModel.FechaFin
                : sessionModel.FechaFin;

            sessionModel.FlagLogin = formModel.FlagLogin;
            sessionModel.FlagBase = formModel.FlagBase;
            sessionModel.FlagAnalisis = formModel.FlagAnalisis;

            // --- CLAVE: NO PISAR IMAGEN SI EL POST NO TRAJO ARCHIVO
            // (en posts intermedios ImagenLoginFile suele venir null)
            if ((sessionModel.ImagenLogin == null || sessionModel.ImagenLogin.Length == 0) &&
                (formModel.ImagenLogin != null && formModel.ImagenLogin.Length > 0))
            {
                sessionModel.ImagenLogin = formModel.ImagenLogin;
            }

            // si el POST no trae imagen, conserva la de sesión
            if (formModel.ImagenLogin == null || formModel.ImagenLogin.Length == 0)
            {
                // no hacer nada: nos quedamos con sessionModel.ImagenLogin
            }
            else
            {
                sessionModel.ImagenLogin = formModel.ImagenLogin;
            }

            return sessionModel;
        }

        private void PersistDraftFormModel()
        {
            var sessionModel = GetSession("NPS_formModel", new EncuestaResponseDTO());

            // OJO: formModel es el que llegó del POST actual (a veces incompleto)
            var merged = sessionModel;

            // Textos/fechas: puedes hacer tu merge normal “si viene vacío, toma de sesión”
            if (!string.IsNullOrWhiteSpace(formModel?.NombreEncuesta)) merged.NombreEncuesta = formModel.NombreEncuesta;
            if (!string.IsNullOrWhiteSpace(formModel?.TituloEncuesta)) merged.TituloEncuesta = formModel.TituloEncuesta;
            if (!string.IsNullOrWhiteSpace(formModel?.TipoPersona)) merged.TipoPersona = formModel.TipoPersona;
            if (formModel?.FechaInicio != null && formModel.FechaInicio != default) merged.FechaInicio = formModel.FechaInicio;
            if (formModel?.FechaFin != null && formModel.FechaFin != default) merged.FechaFin = formModel.FechaFin;

            // ✅ BOOLEANOS: solo actualiza si vinieron en el form POST
            if (Request.Form.ContainsKey("formModel.FlagLogin"))
                merged.FlagLogin = formModel.FlagLogin;

            if (Request.Form.ContainsKey("formModel.FlagBase"))
                merged.FlagBase = formModel.FlagBase;

            if (Request.Form.ContainsKey("formModel.FlagAnalisis"))
                merged.FlagAnalisis = formModel.FlagAnalisis;

            // ✅ IMAGEN: si no vino archivo nuevo, NO la borres
            if (merged.ImagenLogin == null || merged.ImagenLogin.Length == 0)
                merged.ImagenLogin = sessionModel.ImagenLogin;

            SetSession("NPS_formModel", merged);
            formModel = merged;
        }
        public async Task<IActionResult> OnPostEliminarEncuesta(int IdEncuesta)
        {
            try
            {
                RequireTokenOrRedirect();

                // (opcional) cerrar modales por si estaban abiertos
                SetSession("NPS_mostrarModal", false);
                SetSession("NPS_mostrarModalPregunta", false);
                SetSession("NPS_mostrarModalVistaPrevia", false);

                // LLAMADA AL API DELETE
                var ok = await ServiceRepositorio.EliminarEncuesta(token!, IdEncuesta);

                if (!ok)
                {
                    modalTitle = "❌ Error";
                    modalMessage = "No se pudo eliminar la encuesta.";
                }
                else
                {
                    modalTitle = "✅ Éxito";
                    modalMessage = "Encuesta eliminada correctamente.";

                    // Limpia sesiones relacionadas por seguridad
                    HttpContext.Session.Remove("NPS_formModel");
                    HttpContext.Session.Remove("NPS_preguntasDisponibles");
                    HttpContext.Session.Remove("NPS_preguntasEliminadas");
                    HttpContext.Session.Remove("NPS_clientesEncuesta");
                }

                return RedirectToSameListState();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException && ex.Message == "NO_TOKEN")
                    return Redirect("/");

                throw;
            }
        }


    }
}
