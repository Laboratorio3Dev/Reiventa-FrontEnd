using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.NetworkInformation;
using WebBackOffice.DTO.Aprendizaje;
using WebBackOffice.DTO.BackOffice;
using WebBackOffice.Pages.Repositorios;

namespace WebBackOffice.Pages.Aprendizaje
{
    using Aspose.Cells;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;


    using System.Reflection;
    using WebBackOffice.DTO.Aprendizaje;

    [Authorize(Roles = "Admin,Admin Aprendizaje,Gerente Oficina Aprendizaje,Ejecutivo Aprendizaje")]
    public class PlanAccionModel : PageModel
    {
        private readonly BackOfficeLabService _service;

        public PlanAccionModel(
            BackOfficeLabService service)
        {
            _service = service;
        }

        public List<PlanAccionDTO> Todos { get; set; } = new();
        public List<PlanAccionDTO> Planes { get; set; } = new();

        public int Total { get; set; }
        public int NoIniciadas { get; set; }
        public int EnProceso { get; set; }
        public int Atrasadas { get; set; }
        public int Observadas { get; set; }
        public int RevisionGO { get; set; }
        public int Completadas { get; set; }


        [BindProperty(SupportsGet = true, Name = "p")]
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalRegistros => Todos.Count;
        public int TotalPaginas =>
            (int)Math.Ceiling((double)TotalRegistros / PageSize);

        public List<string> ListaColaboradores { get; set; } = new();
        public List<string> ListaProductos { get; set; } = new();
        public List<string> ListaOficinas { get; set; } = new();
        public List<string> ListaZonas { get; set; } = new();
        public List<string> ListaGerentes { get; set; } = new();

        public List<CombosGeneral> Colaboradores = new();
        public List<CombosGeneral> Productos = new();
        public List<CombosGeneral> Dimensiones = new();


        private void CalcularResumen()
        {
            Total = Todos.Count;

            NoIniciadas = Todos.Count(x => x.ESTADO == "No Iniciada");
            EnProceso = Todos.Count(x => x.ESTADO == "En Proceso");
            Atrasadas = Todos.Count(x => x.ESTADO == "Atrasada");
            Observadas = Todos.Count(x => x.ESTADO == "Observada");
            RevisionGO = Todos.Count(x => x.ESTADO == "Revisión GO");
            Completadas = Todos.Count(x => x.ESTADO == "Completada");
        }


        public List<TareasDTO> PopupTareas = new();
        public bool MostrarModal { get; set; } = false; // Nueva propiedad
        [BindProperty] public int? SeleccionProducto { get; set; }
        [BindProperty] public int? SeleccionDimension { get; set; }


        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            var usuarioLogeado = HttpContext.Session.GetString("Usuario");

            Todos = await _service.PlanesAccion(token, usuarioLogeado);

            ListaColaboradores = Todos.Select(x => x.USUARIO).Distinct().OrderBy(x => x).ToList();
            ListaProductos = Todos.Select(x => x.PRODUCTO).Distinct().OrderBy(x => x).ToList();
            ListaOficinas = Todos.Select(x => x.Oficina).Distinct().OrderBy(x => x).ToList();
            ListaZonas = Todos.Select(x => x.Zona).Distinct().OrderBy(x => x).ToList();
            ListaGerentes = Todos.Select(x => x.GerenteOficina).Distinct().OrderBy(x => x).ToList();

            CalcularResumen();

            Planes = Todos
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var datos = await _service.CargarCombosGenerales(token, usuarioLogeado);
            Productos = datos.ListaProductos?.ToList() ?? new List<CombosGeneral>();
            Dimensiones = datos.ListaDimensiones?.ToList() ?? new List<CombosGeneral>();
            Colaboradores = datos.ListaColaboradores?.ToList() ?? new List<CombosGeneral>();
            SeleccionProducto ??= Productos.FirstOrDefault()?.Id;
            SeleccionDimension ??= Dimensiones.FirstOrDefault()?.Id;
        }

        public async Task<JsonResult> OnGetFiltrarAjax(int? productoId, int? dimensionId)
        {
            var token = HttpContext.Session.GetString("Token");
            var tareas = await _service.Tareas(token, productoId, dimensionId);

            // Mapear solo lo necesario para el frontend
            var result = tareas.Select(t => new
            {
                id = t.ID_TAREA,
                nombre = t.TAREA // o la propiedad correcta
            }).ToList();

            return new JsonResult(result);
        }




        public async Task<IActionResult> OnPostExportarExcelAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            var usuario = HttpContext.Session.GetString("Usuario");

            // ✅ USAR await
            List<PlanAccionDTO> planes =
                await _service.PlanesAccion(token, usuario);

            var workbook = new Workbook();
            var sheet = workbook.Worksheets[0];
            sheet.Name = "Planes de Acción";

            sheet.Cells[0, 0].PutValue("Producto");
            sheet.Cells[0, 1].PutValue("Dimensión");
            sheet.Cells[0, 2].PutValue("Tarea");
            sheet.Cells[0, 3].PutValue("Usuario");
            sheet.Cells[0, 4].PutValue("Estado");
            sheet.Cells[0, 5].PutValue("Fecha");

            int row = 1;

            foreach (var item in planes)
            {
                sheet.Cells[row, 0].PutValue(item.PRODUCTO);
                sheet.Cells[row, 1].PutValue(item.DIMENSION);
                sheet.Cells[row, 2].PutValue(item.TAREA);
                sheet.Cells[row, 3].PutValue(item.USUARIO);
                sheet.Cells[row, 4].PutValue(item.ESTADO);
                sheet.Cells[row, 5].PutValue(item.FECHA);
                row++;
            }

            sheet.AutoFitColumns();

            using var ms = new MemoryStream();
            workbook.Save(ms, SaveFormat.Xlsx);
            ms.Position = 0;

            return File(
                ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "PlanesAccion.xlsx"
            );
        }



        [BindProperty]
        public PlanAccionDTO NuevoPlan { get; set; } = new();

        // Acción para el botón "Enviar"
        public async Task<IActionResult> OnPostEnviarGoAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            var usuarioLogeado = HttpContext.Session.GetString("Usuario");

            if (NuevoPlan.ID_PLANACCION > 0)
            {
                var completarGO_DTO = new CompletarGO_DTO
                {
                    IdPlan = NuevoPlan.ID_PLANACCION,
                    Usuario = usuarioLogeado
                };

                var respuesta = await _service.CompletarPlanGO(token, completarGO_DTO);

                if (respuesta.IsSuccess)
                {
                    // Guardamos el mensaje de éxito
                    TempData["MensajeExito"] = "La gestión se ha completado correctamente.";
                    return RedirectToPage();
                }

                ModelState.AddModelError(string.Empty, "Error en la API: " + respuesta.Message);
            }

            // Si falló, recargamos los datos necesarios para la vista (grilla)
            await OnGetAsync();
            return Page();
        }


        public async Task<IActionResult> OnPostGuardarAsignacionesAsync(
     [FromBody] List<CreaTareaDTO> asignaciones)
        {
            if (asignaciones == null || !asignaciones.Any())
            {
                return BadRequest("No hay tareas para asignar");
            }

            var token = HttpContext.Session.GetString("Token");

            var respuesta = await _service.AsignarTareas(token, asignaciones);

            if (respuesta != null && respuesta.IsSuccess)
            {
                return new JsonResult(new
                {
                    success = true,
                    message = "Tareas asignadas correctamente"
                });
            }

            return new JsonResult(new
            {
                success = false,
                message = respuesta?.Message ?? "Error al asignar tareas"
            })
            { StatusCode = 500 };
        }



        public async Task<IActionResult> OnPostAsignarTareaAsync(
                int colaboradorId,
                int productoId,
                int dimensionId,
                int tareaId,
                DateTime fechaLimite
        )
        {
            if (colaboradorId == 0 || tareaId == 0)
            {
                MostrarModal = true;
                await OnGetAsync();   // ✅ RECARGA TODO
                return Page();
            }

            TempData["MensajeExito"] = "Tarea asignada correctamente";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostIniciarPlanAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            // NuevoPlan.ID_PLANACCION ya viene lleno desde el formulario
            if (NuevoPlan.ID_PLANACCION > 0)
            {
                var inicioPlanDTO = new InicioPlanDTO { IdPlan = NuevoPlan.ID_PLANACCION };
                var respuesta = await _service.IniciarPlan(token, inicioPlanDTO);

                if (respuesta.IsSuccess)
                {
                    TempData["MensajeExito"] = "Se inició el plan correctamente.";
                    return RedirectToPage();
                }
                ModelState.AddModelError(string.Empty, respuesta.Message);
            }

            await OnGetAsync(); // Recargar grilla en caso de error
            return Page();
        }


        // Acción para el botón "Observar"


        public async Task<IActionResult> OnPostObservarAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (NuevoPlan.ID_PLANACCION > 0)
            {
                var observarDTO = new RechazoGO_DTO
                {
                    IdPlan = NuevoPlan.ID_PLANACCION,
                    Comentario = NuevoPlan.Comentario,
                    FechaLimite = Convert.ToDateTime(NuevoPlan.FECHA), // La fecha modificada por el GO
                    Usuario = HttpContext.Session.GetString("Usuario")
                };

                var respuesta = await _service.RechazoGO(token, observarDTO);

                if (respuesta.IsSuccess)
                {
                    TempData["MensajeExito"] = "El plan ha sido observado y devuelto al ejecutivo.";
                    return RedirectToPage(new { p = Page });
                }
            }

            return RedirectToPage();
        }



        // Agrega esta propiedad para capturar el archivo del formulario
        [BindProperty]
        public IFormFile ArchivoEvidencia { get; set; }

        public async Task<IActionResult> OnPostGuardarGestionAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            if (NuevoPlan.ID_PLANACCION > 0)
            {
                byte[] fileBytes = null;

                // Convertir archivo a byte[]
                if (ArchivoEvidencia != null && ArchivoEvidencia.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await ArchivoEvidencia.CopyToAsync(ms);
                        fileBytes = ms.ToArray();
                    }
                }

                // Crear DTO para el servicio
                var gestionDTO = new EnvioGO_DTO
                {
                    IdPlan = NuevoPlan.ID_PLANACCION,
                    Comentario = NuevoPlan.Comentario,
                    Evidencia = fileBytes
                };

                var respuesta = await _service.EnviarGO(token, gestionDTO);

                if (respuesta.IsSuccess)
                {
                    TempData["MensajeExito"] = "Gestión guardada correctamente.";
                    return RedirectToPage();
                }
            }

            return Page();
        }


    }


}
