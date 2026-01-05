using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebEncuestaRespuesta.DTO.NPS;
using System.Net.Http.Json;
namespace WebEncuestaRespuesta.Pages.Repositorios
{
    public class NPSService
    {
        private readonly HttpClient _http;

        public NPSService(IHttpClientFactory http)
        {
            _http = http.CreateClient("ApiClient");
        }

        public async Task<EncuestaResponseDTO> ObtenerEncuestaPorId(string token, int idEncuesta)
        {
            var resultado = new EncuestaResponseDTO();

            try
            {
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
        public async Task<bool> GuardarRespuestas(GuardarRespuestasRequest req)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/NPS/Encuesta/GuardarRespuestas", req);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error HTTP {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<ValidacionClienteEncuestaResult> validarClienteEncuesta(
        string usuarioEncriptado,
        string encuestaEncriptada)
        {
            try
            {
                var usuarioToken = (usuarioEncriptado ?? "").Trim().Replace(" ", "+");
                var url = $"api/NPS/Encuesta/ExisteClienteEncuesta";
                var response = await _http.GetFromJsonAsync<ValidacionClienteEncuestaResult>($"{url}?encuesta={Uri.EscapeDataString(encuestaEncriptada)}&u={Uri.EscapeDataString(usuarioEncriptado)}");



                return response ?? new ValidacionClienteEncuestaResult
                {
                    Existe = false,
                    YaRespondio = false
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de conexión al obtener la encuesta: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
            }

            return new ValidacionClienteEncuestaResult
            {
                Existe = false,
                YaRespondio = false
            };
        }
        public async Task<string> obtenerURLClientePorDni(ValidarClienteRequest dniRequest, string valorEncriptado)
        {
            string resultado = "";

            try
            {
                Console.WriteLine(dniRequest);

                var url = "api/NPS/Encuesta/ValidarClienteEncuesta";
                var response = await _http.PostAsJsonAsync(
                    $"{url}?encuesta={Uri.EscapeDataString(valorEncriptado)}",
                    dniRequest);

                if (response.IsSuccessStatusCode)
                {
                    var validarCliente = await response.Content.ReadFromJsonAsync<string>();

                    if (!string.IsNullOrWhiteSpace(validarCliente))
                        resultado = validarCliente.Trim();
                }
                else
                {
                    Console.WriteLine($"Error HTTP {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return resultado;
        }
        public async Task<EncuestaResponseDTO> ObtenerEncuestaPorIdEncriptado(string token, string valorEncriptado)
        {
            var resultado = new EncuestaResponseDTO();

            try
            {

                var url = $"api/NPS/Encuesta/CargarEncuestaEncriptada";
                var response = await _http.GetAsync($"{url}?encuesta={Uri.EscapeDataString(valorEncriptado)}");
                if (response.IsSuccessStatusCode)
                {
                    var encuestas = await response.Content.ReadFromJsonAsync<EncuestaResponseDTO>();
                    if (encuestas != null)
                        resultado = encuestas;
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
    }
}