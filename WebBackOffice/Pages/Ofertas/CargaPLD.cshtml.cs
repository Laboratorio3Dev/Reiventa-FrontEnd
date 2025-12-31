using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Text;
using WebBackOffice.DTO.Ofertas;
using WebBackOffice.Pages.Repositorios;

namespace WebBackOffice.Pages.Ofertas
{
    [Authorize(Roles = "Admin,AdminPLD")]
    public class CargaPLDModel : PageModel
    {

        private readonly BackOfficeLabService _service;

        public CargaPLDModel(
            BackOfficeLabService service)
        {
            _service = service;
        }
        [BindProperty]
        public IFormFile ArchivoTxt { get; set; }

        public string Mensaje { get; set; }
        public bool EsError { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ArchivoTxt == null || ArchivoTxt.Length == 0)
            {
                Mensaje = "Debe seleccionar un archivo TXT.";
                EsError = true;
                return Page();
            }

            try
            {
                var token = HttpContext.Session.GetString("Token");

                var response = await _service.CargarClientesPLD(
                    token,
                    ArchivoTxt
                );

                if (response == null || !response.IsSuccess)
                {
                    Mensaje = "Error durante la carga del archivo.";
                    EsError = true;
                    return Page();
                }

                Mensaje = response.Message;
                EsError = false;
            }
            catch (Exception)
            {
                Mensaje = "Error inesperado durante la carga.";
                EsError = true;
            }

            return Page();
        }

    }
}
