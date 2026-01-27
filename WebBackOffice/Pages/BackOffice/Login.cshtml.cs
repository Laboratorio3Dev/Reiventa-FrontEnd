

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;
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
            HttpContext.Session.SetString("Correo", response.Correo);
            HttpContext.Session.SetString("IdOficina", response.IdOficina.ToString());
            HttpContext.Session.SetString("Token", response.Token);
            HttpContext.Session.SetString("MenuUsuario",JsonSerializer.Serialize(response.MenuUsuario));
         
            var roles = response.RolesUsuario;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, response.UsuarioWindows)
            };

            string nivelAcceso = "Ejecutivo"; // default
            if (roles != null)
            {
                foreach (var rol in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol.NombreRol));
                    if (rol.IdRol == 5) { HttpContext.Session.SetString("Rol", "EsEjecutivo") ;  }
                    if (rol.IdRol == 4) { HttpContext.Session.SetString("Rol", "EsGO") ;  }
                    if (rol.IdRol == 1 || rol.IdRol == 3) { HttpContext.Session.SetString("Rol", "EsAdmin"); }


                    if (roles.Any(r => r.IdRol == 1))
                        nivelAcceso = "Admin";
                    else if (roles.Any(r => r.IdRol == 9))
                        nivelAcceso = "Gerente";
                    else if (roles.Any(r => r.IdRol == 10))
                        nivelAcceso = "Zonal";
                    else if (roles.Any(r => r.IdRol == 8))
                        nivelAcceso = "Ejecutivo";
                }
            }
            HttpContext.Session.SetString("NivelAcceso", nivelAcceso);
            // Crear identidad UNA sola vez
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            // Login
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToPage(response.PaginaPrincipal);
        }

        ErrorMessage = response.ErrorMessage;
        return Page();
    }
}
