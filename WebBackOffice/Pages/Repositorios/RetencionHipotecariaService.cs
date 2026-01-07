using WebBackOffice.DTO.Oficinas;

namespace WebBackOffice.Pages.Repositorios
{
    public class RetencionHipotecariaService
    {
        private readonly HttpClient _http;

        public RetencionHipotecariaService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("ApiClient");
        }

        public async Task<List<RetencionHipotecariaSolicitudDTO>> Listar(
            string usuario,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int page,
            int pageSize)
        {
            var url =
                $"api/RetencionHipotecaria/Listar" +
                $"?usuario={usuario}" +
                $"&fechaInicio={fechaInicio:yyyy-MM-dd}" +
                $"&fechaFin={fechaFin:yyyy-MM-dd}" +
                $"&page={page}" +
                $"&pageSize={pageSize}";

            return await _http.GetFromJsonAsync<List<RetencionHipotecariaSolicitudDTO>>(url)
                   ?? new();
        }
    }
}
