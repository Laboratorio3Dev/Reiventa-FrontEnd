using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebBackOffice.DTO.NPS;

namespace WebBackOffice.Pages.Repositorios
{
    public class NPSService
    {
        private readonly HttpClient _http;

        public NPSService(IHttpClientFactory http)
        {
            _http = http.CreateClient("ApiClient");
        }

        private void EnsureBearer(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("NO_TOKEN");

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<EncuestaResponseDTO>> ObtenerEncuestas(string token, string busquedaNombre)
        {
            EnsureBearer(token);

            var encuestas = await _http.GetFromJsonAsync<List<EncuestaResponseDTO>>("api/NPS/Encuesta");

            if (encuestas == null)
                return new List<EncuestaResponseDTO>();

            if (!string.IsNullOrWhiteSpace(busquedaNombre))
            {
                return encuestas
                    .Where(e => !string.IsNullOrEmpty(e.NombreEncuesta) &&
                                e.NombreEncuesta.Contains(busquedaNombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return encuestas;
        }

        public async Task<EncuestaResponseDTO> ObtenerEncuestaPorId(string token, int idEncuesta)
        {
            var resultado = new EncuestaResponseDTO();

            try
            {
                EnsureBearer(token);

                if (idEncuesta <= 0)
                    throw new ArgumentException("El Id de la encuesta no es válido.");

                var url = $"api/NPS/Encuesta/{idEncuesta}";
                var response = await _http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var encuesta = await response.Content.ReadFromJsonAsync<EncuestaResponseDTO>();
                    if (encuesta != null)
                        resultado = encuesta;
                }
                else
                {
                    Console.WriteLine($"Error HTTP {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de conexión al obtener la encuesta: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
            }

            return resultado;
        }

        public async Task<List<BaseClientesEncuestaDTO>> ObtenerBaseCLientesEncuestas(string token, int idEncuesta)
        {
            EnsureBearer(token);

            var url = $"api/NPS/Encuesta/ClientesBaseEncuesta/{idEncuesta}";
            var baseClienteEncuestas = await _http.GetFromJsonAsync<List<BaseClientesEncuestaDTO>>(url);

            return baseClienteEncuestas ?? new List<BaseClientesEncuestaDTO>();
        }

        public async Task<ExportRespuestasEncuestaDto?> ExportarRespuestasEncuesta(string token, int idEncuesta)
        {
            EnsureBearer(token);

            return await _http.GetFromJsonAsync<ExportRespuestasEncuestaDto>(
                $"api/NPS/Encuesta/ExportarRespuestas?idEncuesta={idEncuesta}");
        }


        public async Task<ResponseTransacciones?> CrearEncuesta(string token, CrearEncuestaDTO request)
        {
            EnsureBearer(token);

            var response = await _http.PostAsJsonAsync("api/NPS/Encuesta/CrearEncuesta", request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ResponseTransacciones>();

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"CrearEncuesta ERROR {response.StatusCode}: {error}");
            return null;
        }

        public async Task<ResponseTransacciones?> ActualizarEncuesta(string token, ModificarEncuestaDTO request)
        {
            EnsureBearer(token);

            var response = await _http.PostAsJsonAsync("api/NPS/Encuesta/ActualizarEncuesta", request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ResponseTransacciones>();

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"ActualizarEncuesta ERROR {response.StatusCode}: {error}");
            return null;
        }

        public async Task<ResponseTransacciones?> CargarClientes(string token, CargarClientesEncuesta request)
        {
            EnsureBearer(token);

            var response = await _http.PostAsJsonAsync("api/NPS/Encuesta/CargarClientes", request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ResponseTransacciones>();

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"CargarClientes ERROR {response.StatusCode}: {error}");
            return null;
        }
    }
}