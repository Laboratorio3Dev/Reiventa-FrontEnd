using System.Net.Http.Headers;
using WebBackOffice.DTO.Oficinas;

namespace WebBackOffice.Pages.Repositorios
{
    public class HoudiniServices
    {
        private readonly HttpClient _http;

        public HoudiniServices(IHttpClientFactory http)
        {
            _http = http.CreateClient("ApiClient");
        }

        public async Task<List<ProductoResponseDTO>> ObtenerProductos(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var productos =
                await _http.GetFromJsonAsync<List<ProductoResponseDTO>>(
                    "api/Oficinas/AdminHoudini"
                );

            return productos ?? new List<ProductoResponseDTO>();
        }

        public async Task<bool> RegistrarVentaDigital(
    string token,
    RegistrarVentaDigitalRequest request)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.PostAsJsonAsync(
                "api/VentaDigital/Registrar",
                request
            );
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                Console.WriteLine("❌ ERROR API VentaDigital");
                Console.WriteLine($"Status: {(int)response.StatusCode} - {response.StatusCode}");
                Console.WriteLine($"Body: {error}");

                return false;
            }
            return response.IsSuccessStatusCode;
        }

    }
}
