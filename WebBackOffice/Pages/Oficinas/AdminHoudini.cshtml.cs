using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebBackOffice.DTO.Aprendizaje;
using WebBackOffice.DTO.Oficinas;
using WebBackOffice.Pages.Repositorios;
using WebBackOffice.ViewModels.Oficinas;

namespace WebBackOffice.Pages.Oficinas
{
    public class AdminHoudiniModel : PageModel
    {
       
        private readonly AdminHoudiniServices _service;
   
        public AdminHoudiniModel(AdminHoudiniServices service)
        {
            _service = service;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public List<ProductoVM> PagedProductos { get; set; } = new();


        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;

        public bool CanGoPrevious => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;
        public async Task OnGetAsync(int pageNumber = 1)
        {
            await CargarProductosAsync(pageNumber);
        }

        public async Task<IActionResult> OnPostGuardarProductoAsync(ProductoDTO producto)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new ResponseTransacciones
                {
                    IsSuccess = false,
                    Message = "Datos inválidos"
                });

            var token = HttpContext.Session.GetString("Token");
         
            ResponseTransacciones response;

            if (producto.IdProducto.HasValue)
            {
                response = await _service.ActualizarProducto(token, producto);
            }
            else
            {
                response = await _service.GuardarProducto(token, producto);
            }

            return new JsonResult(response);
        }

        public async Task<IActionResult> OnGetObtenerProductoAsync(int id)
        {
            var token = HttpContext.Session.GetString("Token");

            var producto = await _service.ObtenerProductoPorId(token, id);

            if (producto == null)
                return NotFound();

            return new JsonResult(producto);
        }

       
        public async Task<PartialViewResult> OnGetTablaProductosAsync(
      int pageNumber,
      string? searchTerm)
        {
            SearchTerm = searchTerm;
            await CargarProductosAsync(pageNumber);

            return Partial("_TablaProductos", this);
        }



        private async Task CargarProductosAsync(int pageNumber)
        {
        
            var token = HttpContext.Session.GetString("Token");

            var productosDto = await _service.ObtenerProductos(token);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                productosDto = productosDto
                    .Where(p => p.Titulo.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            TotalPages = (int)Math.Ceiling(productosDto.Count / (double)PageSize);
            CurrentPage = pageNumber;

            PagedProductos = productosDto
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                 .Select(p => new ProductoVM
                 {
                     IdProducto = p.IdProducto,
                     Titulo = p.Titulo,
                     SubTitulo = p.SubTitulo,
                     Asunto = p.Asunto,
                     Activo = p.Activo
                 })
                .ToList();
        }

        public async Task<IActionResult> OnPostCambiarEstadoProductoAsync(
    [FromBody] CambiarEstadoRequest request)
        {
            var token = HttpContext.Session.GetString("Token");

            var response = await _service.CambiarEstadoProducto(
                token,
                request.IdProducto,
                request.Activar
            );

            return new JsonResult(response);
        }

    }
}
