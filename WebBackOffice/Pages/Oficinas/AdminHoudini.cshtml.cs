using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebBackOffice.DTO.Oficinas;
using WebBackOffice.Pages.Repositorios;
using WebBackOffice.ViewModels.Oficinas;

namespace WebBackOffice.Pages.Oficinas
{
    public class AdminHoudiniModel : PageModel
    {
        bool mostrarModal = false;
        string modoModal = "nuevo"; // nuevo | editar
        bool EsSoloLectura => modoModal == "ver";
        ProductoVM formModel;
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

        public bool CanGoPrevious => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;

        public async Task OnGetAsync(int pageNumber = 1)
        {
            var token = HttpContext.Session.GetString("Token");
            CurrentPage = pageNumber;

            var productosDto = await _service.ObtenerProductos(token);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                productosDto = productosDto
                    .Where(x => x.Titulo.Contains(SearchTerm,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int pageSize = 10;
            TotalPages = (int)Math.Ceiling(productosDto.Count / (double)pageSize);

            PagedProductos = productosDto
                .Skip((CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductoVM
                {
                    IdProducto = p.IdProducto,
                    Titulo = p.Titulo,
                    SubTitulo = p.SubTitulo,
                    Asunto = p.Asunto
                })
                .ToList();
        }

        public async Task<IActionResult> OnPostGuardarProductoAsync(ProductoDTO producto)
        {
            if (producto == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Producto llegó null"
                });
            }

            if (string.IsNullOrWhiteSpace(producto.Titulo))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Título obligatorio"
                });
            }

            var token = HttpContext.Session.GetString("Token");

            var response = await _service.GuardarProducto(token, producto);

            return new JsonResult(response);
        }




    }
}
