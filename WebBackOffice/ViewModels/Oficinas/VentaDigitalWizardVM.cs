using System.ComponentModel.DataAnnotations;

namespace WebBackOffice.ViewModels.Oficinas
{
    using System.ComponentModel.DataAnnotations;

    namespace WebBackOffice.ViewModels.Oficinas
    {
        public class VentaDigitalWizardVM
        {
            // === CONTROL DEL PASO ===
            public int Paso { get; set; } = 1;

            // === DATOS CLIENTE (PASO 1) ===
            [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
            public string NombreCliente { get; set; }

            [Required(ErrorMessage = "El DNI es obligatorio")]
            public string DocumentoCliente { get; set; }

            [Required(ErrorMessage = "El correo es obligatorio")]
            [EmailAddress(ErrorMessage = "Correo inválido")]
            public string CorreoCliente { get; set; }

            [Required(ErrorMessage = "La oficina es obligatoria")]
            public string CodOficina { get; set; }

            [Required(ErrorMessage = "El código de vendedor es obligatorio")]
            public string CodigoVendedor { get; set; }

            // === PASO 2 ===
            public List<int> ProductosSeleccionados { get; set; } = new();
        }
    }

}
