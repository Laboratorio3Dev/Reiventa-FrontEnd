using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.ViewModels.Oficinas;

namespace WebBackOffice.Pages.Oficinas
{
    public class _GestionClienteModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetGestionAsync(string documento)
        {
            //var data = await _servicio.ObtenerGestionCliente(documento);

            var vm = new GestionClienteVM();
            //{
            //    Documento = data.DOCUMENTO,
            //    Celular = data.CELULAR,
            //    Estado = data.ESTADO
            //};

            return Partial("_GestionCliente", vm);
        }

    }
}
