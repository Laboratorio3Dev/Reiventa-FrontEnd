using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using WebBackOffice.DTO.Oficinas;
using WebBackOffice.Helper;
using WebBackOffice.Infrastructure;
using WebBackOffice.Pages.Repositorios;
using WebBackOffice.ViewModels.Oficinas;

namespace WebBackOffice.Pages.Oficinas
{
    public class NegocioHipotecarioModel : BasePageModel
    {
        private readonly RetencionHipotecariaService _service;
     

        public NegocioHipotecarioModel(RetencionHipotecariaService service)
        {
            _service = service;
        }
        public DashboardResumenVM Resumen { get; set; } = new();
        public List<ListadoHipotecarioVM> Listado { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime FechaInicio { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime FechaFin { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Estado { get; set; }

        public void OnGet()
        {
            if (FechaInicio == default)
                FechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            if (FechaFin == default)
                FechaFin = DateTime.Today;

        }

     

        public async Task<IActionResult> OnGetListarAsync(
            DateTime fechaInicio,
            DateTime fechaFin,
            string? estado,
            int pageNumber,
            int PageSize = 10)
        {
            var token = HttpContext.Session.GetString("Token");
            var usuario = HttpContext.Session.GetString("Usuario");
            var nivel = HttpContext.Session.GetString("NivelAcceso");
            var request = new ListarNegocioHipotecarioRequest
            {
                Usuario = usuario!,
                NivelAcceso = nivel,
                Estado = estado,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Page = pageNumber,
                PageSize = PageSize,
                
            };
            var result = await _service.ListarNegocioHipotecario(request, token);

        
            var resumen = new DashboardResumenVM
            {
                Total = result.Total,
                SinGestion = result.Items.Count(x => x.ESTADO == "Sin Gestión"),
                Interesados = result.Items.Count(x => x.ESTADO == "Interesado"),
                EnSeguimiento = result.Items.Count(x => x.ESTADO == "En Seguimiento"),
                Rechazado = result.Items.Count(x => x.ESTADO == "Rechazado"),
                NoContactable = result.Items.Count(x => x.ESTADO == "No Contactable"),
                Aprobado = result.Items.Count(x => x.ESTADO == "Aprobado"),
                NoInteresado = result.Items.Count(x => x.ESTADO == "No Interesado"),
                Observado = result.Items.Count(x => x.ESTADO == "Observado")
            };

            return new JsonResult(new NegocioHipotecarioResponse
            {
                Listado = result,
                Resumen = resumen,
                Total = result.Total
            });
        }


        public async Task<IActionResult> OnGetGestionAsync(int id, int codigoCliente, string documento, string celular, string estado,string comentario)
        {
           
            var token = HttpContext.Session.GetString("Token");
            var simulaciones = await _service .ListarPorCliente(codigoCliente, token);
            var vm = new GestionClienteVM
            {
                Id = id,
                Documento = documento,
                Celular = celular,
                Estado = estado,
                Comentarios = comentario,
                Simulaciones = simulaciones.Select(x => new SimulacionHipotecariaVM
                {
                    Moneda = x.Moneda,
                    ValorInmueble = x.ValorInmueble,
                    MontoInicial = x.MontoInicial,
                    DineroNecesita = x.DineroNecesita,
                    IngresoMensual = x.IngresoMensual,
                    TipoIngreso = x.TipoIngreso,
                    ComparteCuota = x.ComparteCuota,
                    ConoceInmueble = x.ConoceInmueble,
                    PrimeraVivienda = x.PrimeraVivienda
                }).ToList()
            };
            return Partial("_GestionCliente", vm);
        }

        public async Task<IActionResult> OnPostGuardarGestionAsync(ActualizarGestionClienteRequest command)
        {
            var token = HttpContext.Session.GetString("Token");

            var result = await _service.ActualizarGestion(command, token);

            return new JsonResult(result);
        }


        public async Task<IActionResult> OnGetDescargarExcelAsync(DateTime fechaInicio,DateTime fechaFin,string? estado)
        {
            var token = HttpContext.Session.GetString("Token");

            var request = new ListarNegocioHipotecarioRequest
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Estado = estado,
                Page = 1,
                PageSize = int.MaxValue // 🔥 todo para el Excel
            };

            var result = await _service.ListarNegocioHipotecario(request, token);

            // 🔹 MAPEO DTO → VM
            var listadoVm = result.Items.Select(x => new ListadoHipotecarioVM
            {
                Ejecutivo = x.EJECUTIVO,
                Documento = x.DOCUMENTO,
                Celular = x.CELULAR,
                Correo = x.CORREO,
                Score = x.SCORE,
                Fecha = x.FECHA,
                Estado = x.ESTADO
            }).ToList();

            var excelBytes = ExcelHelper.GenerarExcelNegocioHipotecario(listadoVm);

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"NegocioHipotecario_{DateTime.Now:yyyyMMddHHmm}.xlsx"
            );
        }

    }
}
