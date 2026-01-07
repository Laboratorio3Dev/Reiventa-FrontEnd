using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.Pages.Repositorios;

namespace WebBackOffice.Pages.Oficinas
{
    public class CargaRetencionHipotecariaModel : PageModel
    {
        private readonly RetencionServices _service;

        public CargaRetencionHipotecariaModel(RetencionServices service)
        {
            _service = service;
        }
 
    
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCargarExcelAsync(IFormFile Archivo)
        {
            if (Archivo == null || Archivo.Length == 0)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = "Debe seleccionar un archivo"
                });
            }

            var token = HttpContext.Session.GetString("Token");

            var response = await _service.CargarExcel(token, Archivo);

            return new JsonResult(response);
        }
    }
}
