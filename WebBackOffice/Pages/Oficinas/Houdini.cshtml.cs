using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using WebBackOffice.DTO.Oficinas;
using WebBackOffice.Pages.Repositorios;
using WebBackOffice.ViewModels.Oficinas;
using WebBackOffice.ViewModels.Oficinas.WebBackOffice.ViewModels.Oficinas;
using static WebBackOffice.Pages.Aprendizaje.DashboardModel;

namespace WebBackOffice.Pages.Oficinas
{
    public class HoudiniModel : PageModel
    {
        private readonly HoudiniServices _service;
        [BindProperty]
        public VentaDigitalWizardVM Wizard { get; set; } = new();

        public string Token { get; set; }
        public string Usuario { get; set; }

        public List<ProductoVM> Productos { get; set; } = new();

        public HoudiniModel(HoudiniServices service)
        {
            _service = service;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Token = HttpContext.Session.GetString("Token");
            Usuario = HttpContext.Session.GetString("Usuario");

            var dtos = await _service.ObtenerProductos(Token);

            Productos = dtos.Select(Mapear).ToList();

            return Page();
        }

        private ProductoVM Mapear(ProductoResponseDTO dto)
        {
            return new ProductoVM
            {
                IdProducto = dto.IdProducto,
                Titulo = dto.Titulo,
                SubTitulo = dto.SubTitulo
            };
        }

        //public async Task<IActionResult> OnPostSiguienteAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        await CargarProductos();
        //        return Page();
        //    }

        //    Wizard.Paso = 2;
        //    await CargarProductos();
        //    return Page();
        //}
        public IActionResult OnPostValidarCliente()
        {
            var nombre = Request.Form["NombreCliente"].ToString();
            var documento = Request.Form["DocumentoCliente"].ToString();
            var correo = Request.Form["CorreoCliente"].ToString();
         
            var vendedor = Request.Form["CodigoVendedor"].ToString();

            // Validación general + formato de correo
            if (
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(documento) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(vendedor) ||
                !CorreoValido(correo)
            )
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "Debe completar todos los datos obligatorios del cliente con información válida"
                });
            }
            // Guardar en sesión
            HttpContext.Session.SetString("NombreCliente", nombre);
            HttpContext.Session.SetString("DocumentoCliente", documento);
            HttpContext.Session.SetString("CorreoCliente", correo);
            HttpContext.Session.SetString("CodigoVendedor", vendedor);
            return new JsonResult(new
            {
                ok = true,
                mensaje = "Datos del cliente validados correctamente"
            });
        }

        public async Task<IActionResult> OnPostEnviarAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            var Username = HttpContext.Session.GetString("Username");
            var NombreCompleto = HttpContext.Session.GetString("NombreCliente");
            var Correo = HttpContext.Session.GetString("CorreoCliente");
            var IdOficina = HttpContext.Session.GetString("IdOficina");
            var CodigoVendedor = HttpContext.Session.GetString("CodigoVendedor");
            var DocumentoCliente = HttpContext.Session.GetString("DocumentoCliente");

            var idsSeleccionados = Request.Form["ProductosSeleccionados"]
                .Select(int.Parse)
                .ToList();

            // ❌ VALIDACIÓN
            if (!idsSeleccionados.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un producto");
                await CargarProductos();
                return Page();
            }

            var request = new RegistrarVentaDigitalRequest
            {
                ProductosSeleccionados = idsSeleccionados,
                CorreoCliente = Correo,    
                UsuarioRegistro = Username,

                DocumentoCliente = DocumentoCliente,
                NombreCliente = NombreCompleto,
                CodigoVendedor = CodigoVendedor,
                CodOficina = IdOficina
            };

            var ok = await _service.RegistrarVentaDigital(token, request);

            return RedirectToPage("/Oficinas/HoudiniExito");

        }

        private async Task CargarProductos()
        {
            var token = HttpContext.Session.GetString("Token");
            var dtos = await _service.ObtenerProductos(token);
            Productos = dtos.Select(Mapear).ToList();
        }

        private bool CorreoValido(string correo)
        {
            try
            {
                var addr = new MailAddress(correo);
                return addr.Address == correo;
            }
            catch
            {
                return false;
            }
        }
    }
}
