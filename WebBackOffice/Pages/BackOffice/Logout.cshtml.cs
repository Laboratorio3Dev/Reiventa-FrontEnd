using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebBackOffice.Pages.BackOffice
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // 🔴 Limpiar toda la sesión
            HttpContext.Session.Clear();

            // 🔴 Eliminar cookie de sesión
            Response.Cookies.Delete(".AspNetCore.Session");

            // 🔁 Redirigir al login
            return RedirectToPage("/BackOffice/Login");
        }
    }
}
