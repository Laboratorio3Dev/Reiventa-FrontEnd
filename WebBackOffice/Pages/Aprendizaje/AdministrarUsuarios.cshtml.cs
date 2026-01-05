using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.DTO.Aprendizaje;
using WebBackOffice.DTO.BackOffice;
using WebBackOffice.Pages.Repositorios;

namespace WebBackOffice.Pages.Aprendizaje
{
    public class AdministrarUsuariosModel : PageModel
    {
        private readonly BackOfficeLabService _service;

        public AdministrarUsuariosModel(
            BackOfficeLabService service)
        {
            _service = service;
        }

        public List<UsuarioDto> Usuarios { get; set; } = new();
        public List<string> Roles { get; set; }
        public List<string> Oficinas { get; set; }

        [BindProperty]
        public UsuarioDto Usuario { get; set; }

        [BindProperty]
        public string DniReset { get; set; }

        private async Task CargarDatos()
        {
            var token = HttpContext.Session.GetString("Token");

            Usuarios = await _service.ListarUsuarios(token, 1);

            Roles = Usuarios
                .Where(x => !string.IsNullOrEmpty(x.Rol))
                .Select(x => x.Rol)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            Oficinas = Usuarios
               .Where(x => !string.IsNullOrEmpty(x.Oficina))
               .Select(x => x.Oficina)
               .Distinct()
               .OrderBy(x => x)
               .ToList();
        }
        public async Task OnGetAsync()
        {
            await CargarDatos();
        }

        public async Task<IActionResult> OnPostCrearAsync()
        {
            //await _api.PostAsync("usuarios", Usuario);
            //return RedirectToPage();
            return Page();
        }

        public async Task<IActionResult> OnPostEditarAsync()
        {
            //await _api.PutAsync($"usuarios/{Usuario.IdUsuario}", Usuario);
            //return RedirectToPage();
            return Page();
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(int idUsuario)
        {
            //await _api.PostAsync($"usuarios/{idUsuario}/reset-password",
            //    new { dni = DniReset });

            //return RedirectToPage();
            return Page();
        }
    }
}
