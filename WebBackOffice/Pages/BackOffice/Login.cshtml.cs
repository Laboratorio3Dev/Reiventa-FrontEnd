

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.DTO.BackOffice;
using WebBackOffice.Pages.Repositorios;

public class LoginModel : PageModel
{
    private readonly BackOfficeLabService _service;

    public LoginModel(BackOfficeLabService service)
    {
        _service = service;
    }

    [BindProperty]
    public string Usuario { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var response = await _service.LoginAsync(Usuario, Password);

        if (response.IsSuccess)
        {
            // 🔐 Guardar sesión (ejemplo simple)
            HttpContext.Session.SetString("Username", response.NombreCompleto);
            HttpContext.Session.SetString("Usuario", response.UsuarioWindows);
            HttpContext.Session.SetString("Token", response.Token);

            var roles = response.RolesUsuario;

            if (roles != null)
            {
                foreach (var rol in roles)
                {
                    if (rol.IdRol == 5) { HttpContext.Session.SetString("Rol", "EsEjecutivo") ;  }
                    if (rol.IdRol == 4) { HttpContext.Session.SetString("Rol", "EsGO") ;  }
                    if (rol.IdRol == 1 || rol.IdRol == 3) { HttpContext.Session.SetString("Rol", "EsAdmin"); }
                }
            }
            
            return RedirectToPage("/Aprendizaje/Dashboard");
        }

        ErrorMessage = response.ErrorMessage;
        return Page();
    }
}
