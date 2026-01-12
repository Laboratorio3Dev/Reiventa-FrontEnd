using WebBackOffice.DTO.Common;
using WebBackOffice.ViewModels.Oficinas;

namespace WebBackOffice.DTO.Oficinas
{
    public class RetencionListadoResponse
    {
        public PagedResult<RetencionHipotecariaSolicitudDTO> Listado { get; set; }
        public List<EstadoResumenVM> Estados { get; set; }
    }
}
