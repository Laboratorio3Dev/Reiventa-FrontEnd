using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.DTO.Common;
using WebBackOffice.DTO.Oficinas;
using WebBackOffice.Pages.Repositorios;
using WebBackOffice.ViewModels.Oficinas;

namespace WebBackOffice.Pages.Oficinas
{
    public class RetencionHipotecariaModel : PageModel
    {
        private readonly RetencionHipotecariaService _service;

        public RetencionHipotecariaModel(RetencionHipotecariaService service)
        {
            _service = service;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnGetListarAsync(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int pageNumber,
            int PageSize = 10)
        {
            var token = HttpContext.Session.GetString("Token");
            var usuario = HttpContext.Session.GetString("Usuario");

            var request = new ListarRetencionHipotecariaRequest
            {
                // Usuario solo si tu API lo exige
                Usuario = usuario,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Page = pageNumber,
                PageSize = PageSize
            };
            var result = await _service.Listar(
               request, token
            );

            foreach (var item in result.Items)
            {
                item.MONEDA = item.MONEDA switch
                {
                    "S" => "Soles",
                    "D" => "Dólares",
                    _ => "No definido"
                };
            }

            var estados = result.Items
            .GroupBy(x => x.ESTADO)
            .Select(g => new EstadoResumenVM
            {
                Estado =(int) g.Key,
                EstadoTexto = g.Key switch
                {
                    1 => "APROBADO",
                    2 => "PROPUESTA",
                    3 => "NO APLICA",
                    _ => "OTRO"
                },
                Cantidad = g.Count(),
                Porcentaje = Math.Round(
                    g.Count() * 100.0 / result.Items.Count, 0)
            }).ToList();
            return new JsonResult(new RetencionListadoResponse
            {
                Listado = result,
                Estados = estados
            });
        }

        public async Task<IActionResult> OnGetListarProductoAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            var result = await _service.ListarProductosVincular(token);

            return new JsonResult(result);

        }

        public async Task<IActionResult> OnGetListarEntidadesAsync()
        {
            var token = HttpContext.Session.GetString("Token");

            var result = await _service.ListarEntidades(token);

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostGuardarAsync(RetencionHipotecariaGuardarDTO dto)
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            var token = HttpContext.Session.GetString("Token");
            if (dto == null)
            {
                return new JsonResult(new ResponseTransacciones
                {
                    IsSuccess = false,
                    Message = "Datos inválidos"
                });
            }
            dto.Usuario = usuario;
            var result = await _service.GuardarSolicitud(dto, token);
            return new JsonResult(result);
        }
    }
}
