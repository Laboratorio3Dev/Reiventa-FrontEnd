using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebBackOffice.Pages.BackOffice
{
    public class AccesoDenegadoModel : PageModel
    {
        [AllowAnonymous]
        public void OnGet()
        {
        }
    }
}
