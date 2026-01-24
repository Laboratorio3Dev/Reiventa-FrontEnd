using WebBackOffice.DTO.Common;
using WebBackOffice.ViewModels.Oficinas;

namespace WebBackOffice.DTO.Oficinas
{
    public class NegocioHipotecarioResponse
    {
        public DashboardResumenVM Resumen { get; set; } = new();
        public PagedResult<ListadoHipotecarioDTO> Listado { get; set; } = new();
        public int Total { get; set; }
    }
}
