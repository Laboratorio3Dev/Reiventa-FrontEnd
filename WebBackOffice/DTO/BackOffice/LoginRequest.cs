using System.ComponentModel.DataAnnotations;

namespace WebBackOffice.DTO.BackOffice
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; }
    }
}
