using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBackOffice.DTO.Ofertas;
using WebBackOffice.Pages.Repositorios;

namespace WebBackOffice.Pages.Ofertas
{
    [Authorize(Roles = "Admin,AdminPLD,EjecutivoPLD")]
    public class PLDModel : PageModel
    {

        private readonly BackOfficeLabService _service;

        public PLDModel(
            BackOfficeLabService service)
        {
            _service = service;
        }

        [BindProperty]
        public string Dni { get; set; }

        public List<OfertaPLD> Ofertas { get; set; }

        public string Mensaje { get; set; }

        public void OnGet()
        {
            // Pantalla inicial
        }

        public async Task OnPostAsync()
        {
           
            var token = HttpContext.Session.GetString("Token");
            var usuarioLogeado = HttpContext.Session.GetString("Usuario");
            OfertaRequest ofertaRequest = new OfertaRequest();
            ofertaRequest.Usuario = usuarioLogeado;
            ofertaRequest.Documento = Dni;

            var datos = await _service.ValidarOfertaPLD(token, ofertaRequest);

            if (datos != null)
            {
                Ofertas = new List<OfertaPLD>
            {
                new OfertaPLD { Plazo = 12, Monto = datos.OF_S_PLDD_12, Tasa = datos.TASA_S_PLDD_12_FIN },
                new OfertaPLD { Plazo = 18, Monto = datos.OF_S_PLDD_18, Tasa = datos.TASA_S_PLDD_18_FIN },
                new OfertaPLD { Plazo = 24, Monto = datos.OF_S_PLDD_24, Tasa = datos.TASA_S_PLDD_24_FIN },
                new OfertaPLD { Plazo = 36, Monto = datos.OF_S_PLDD_36, Tasa = datos.TASA_S_PLDD_36_FIN }
            };
            }
            else
            {
                Mensaje = "El cliente no cuenta con oferta en esta campaña.";
            }
        }
    }
}