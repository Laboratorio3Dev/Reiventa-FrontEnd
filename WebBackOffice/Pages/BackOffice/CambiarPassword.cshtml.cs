using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebBackOffice.Pages.Repositorios;
using WebBackOffice.DTO.BackOffice;

namespace WebBackOffice.Pages.BackOffice
{
    public class CambiarPasswordModel : PageModel
    {
        private readonly BackOfficeLabService _service;

        public CambiarPasswordModel(BackOfficeLabService service)
        {
            _service = service;
        }

        [BindProperty]
        public PasswordModel Password { get; set; } = new();

        public string Mensaje { get; set; }
        public bool EsExito { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Password.PasswordNueva != Password.PasswordConfirmacion)
            {
                Mensaje = "La nueva contraseña y la confirmación no coinciden.";
                EsExito = false;
                return Page();
            }

            var usuario = HttpContext.Session.GetString("Usuario");
            var token = HttpContext.Session.GetString("Token");

            var request = new CambiarPasswordRequest
            {
                Usuario = usuario,
                Password_Old = Password.PasswordActual,
                Password_New = Password.PasswordNueva
            };

            var response = await _service.CambiarPassword(token, request);

            if (!response.IsSuccess)
            {
                Mensaje = "No se pudo actualizar la contraseña.";
                EsExito = false;
                return Page();
            }

            EsExito = true;
            Mensaje = "Contraseña actualizada correctamente.";
            ModelState.Clear();
            Password = new PasswordModel();

            return Page();
        }

        public class PasswordModel
        {
            [Required(ErrorMessage = "Ingrese su contraseña actual.")]
            public string PasswordActual { get; set; }

            [Required]
            [MinLength(8)]
            [RegularExpression(
                "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+=\\-{};:,.<>?]).+$",
                ErrorMessage = "Debe incluir mayúscula, minúscula, número y carácter especial."
            )]
            public string PasswordNueva { get; set; }

            [Required(ErrorMessage = "Confirme su nueva contraseña.")]
            public string PasswordConfirmacion { get; set; }
        }
    }
}
