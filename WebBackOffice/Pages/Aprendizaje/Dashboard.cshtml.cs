using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using WebBackOffice.DTO.Aprendizaje;
using WebBackOffice.Pages.Repositorios;

namespace WebBackOffice.Pages.Aprendizaje
{
    public class DashboardModel : PageModel
    {
        private readonly BackOfficeLabService _service;

        public DashboardModel(BackOfficeLabService service)
        {
            _service = service;
        }

        // 🔐 Sesión
        public string Token { get; set; }
        public string Usuario { get; set; }

        // 📊 Data base
        public List<ListadoDashboard_DTO> Data { get; set; } = new();
        public List<ProductoInsightDTO> Comentarios { get; set; } = new();

        // 📊 Resúmenes
        public List<EjecutivoResumenVM> Ejecutivos { get; set; } = new();
        public List<ProductoResumenVM> Productos { get; set; } = new();

        public double CumplimientoGeneral { get; set; }

        // 🎯 Filtros (GET)
        [BindProperty(SupportsGet = true)] public int? Anio { get; set; }
        [BindProperty(SupportsGet = true)] public int? Mes { get; set; }
        [BindProperty(SupportsGet = true)] public string? Ejecutivo { get; set; }
        [BindProperty(SupportsGet = true)] public string? Zona { get; set; }
        [BindProperty(SupportsGet = true)] public string? Oficina { get; set; }
        [BindProperty(SupportsGet = true)] public string? Producto { get; set; }
        [BindProperty(SupportsGet = true)] public string? Dimension { get; set; }

        // 📋 Listas filtros
        public List<int> Anios { get; set; } = new();
        public List<int> Meses { get; set; } = new();
        public List<string> EjecutivosFiltro { get; set; } = new();
        public List<string> Zonas { get; set; } = new();
        public List<string> Oficinas { get; set; } = new();
        public List<string> ProductosFiltro { get; set; } = new();
        public List<string> Dimensiones { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Token = HttpContext.Session.GetString("Token");
            Usuario = HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(Token))
                return RedirectToPage("/Index");

            var request = new RequestDashBoard
            {
                Anio = Anio,
                Mes = Mes,
                Usuario = Usuario
            };

            Data = await _service.DatosDashboard(Token, request);

            CargarFiltros();
            AplicarFiltros();
            CalcularResumenes(Data);

            if (!string.IsNullOrEmpty(Ejecutivo))
            {
                var req = new RequestComentarios
                {
                    Anio = Anio,
                    Mes = Mes,
                    Usuario = Ejecutivo
                };

                Comentarios = await _service.ListarComentarios(Token, req);
            }

            return Page();
        }

        private void CargarFiltros()
        {
            Anios = Data.Select(x => x.ANIO).Distinct().OrderByDescending(x => x).ToList();
            Meses = Data.Select(x => x.MES).Distinct().OrderBy(x => x).ToList();
            EjecutivosFiltro = Data.Select(x => x.EJECUTIVO).Distinct().OrderBy(x => x).ToList();
            Zonas = Data.Select(x => x.ZONA).Distinct().OrderBy(x => x).ToList();
            Oficinas = Data.Select(x => x.OFICINA).Distinct().OrderBy(x => x).ToList();
            ProductosFiltro = Data.Select(x => x.PRODUCTO).Distinct().OrderBy(x => x).ToList();
            Dimensiones = Data.Select(x => x.DIMENSION).Distinct().OrderBy(x => x).ToList();
        }

        private void AplicarFiltros()
        {
            var q = Data.AsQueryable();

            if (Anio.HasValue) q = q.Where(x => x.ANIO == Anio);
            if (Mes.HasValue) q = q.Where(x => x.MES == Mes);
            if (!string.IsNullOrEmpty(Ejecutivo)) q = q.Where(x => x.EJECUTIVO == Ejecutivo);
            if (!string.IsNullOrEmpty(Zona)) q = q.Where(x => x.ZONA == Zona);
            if (!string.IsNullOrEmpty(Oficina)) q = q.Where(x => x.OFICINA == Oficina);
            if (!string.IsNullOrEmpty(Producto)) q = q.Where(x => x.PRODUCTO == Producto);
            if (!string.IsNullOrEmpty(Dimension)) q = q.Where(x => x.DIMENSION == Dimension);

            Data = q.ToList();
        }

        private void CalcularResumenes(List<ListadoDashboard_DTO> origen)
        {
            Ejecutivos = origen
                .GroupBy(x => x.EJECUTIVO)
                .Select(g => new EjecutivoResumenVM
                {
                    Ejecutivo = g.Key,
                    Porcentaje = Math.Round(
                        g.Count(x => x.CUMPLIMIENTO == 1) * 100.0 / g.Count(), 0)
                }).ToList();

            Productos = origen
                .GroupBy(x => x.PRODUCTO)
                .Select(g => new ProductoResumenVM
                {
                    Producto = g.Key,
                    Porcentaje = Math.Round(
                        g.Count(x => x.CUMPLIMIENTO == 1) * 100.0 / g.Count(), 0)
                }).ToList();

            CumplimientoGeneral = origen.Any()
                ? Math.Round(origen.Count(x => x.CUMPLIMIENTO == 1) * 100.0 / origen.Count, 0)
                : 0;
        }

        public class EjecutivoResumenVM
        {
            public string Ejecutivo { get; set; } = "";
            public double Porcentaje { get; set; }
        }

        public class ProductoResumenVM
        {
            public string Producto { get; set; } = "";
            public double Porcentaje { get; set; }
        }

        [BindProperty]
        public IFormFile ArchivoExcel { get; set; }

        public async Task<IActionResult> OnPostCargarExcelAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            var usuario = HttpContext.Session.GetString("Usuario");

            if (ArchivoExcel == null || ArchivoExcel.Length == 0)
            {
                TempData["MensajeError"] = "Debe seleccionar un archivo Excel.";
                return RedirectToPage();
            }

            var respuesta = await _service.CargarExcel(
                token,
                ArchivoExcel,
                usuario
            );

            if (respuesta?.IsSuccess == true)
            {
                TempData["MensajeExito"] = "Archivo cargado correctamente.";
            }
            else
            {
                TempData["MensajeError"] = "Error al cargar el archivo.";
            }

            return RedirectToPage();
        }


    }
}
