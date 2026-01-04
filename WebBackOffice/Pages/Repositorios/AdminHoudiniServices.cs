using System.Net.Http.Headers;
using WebBackOffice.DTO.Aprendizaje;
using WebBackOffice.DTO.Oficinas;
using static System.Net.WebRequestMethods;

namespace WebBackOffice.Pages.Repositorios
{
    public class AdminHoudiniServices
    {
        private readonly HttpClient _http;

        public AdminHoudiniServices(IHttpClientFactory http)
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

        public async Task<ResponseTransacciones> GuardarProducto(
    string token,
    ProductoDTO producto)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.PostAsJsonAsync(
               "api/Oficinas/AdminHoudini/Crear",
                producto
            );

            if (!response.IsSuccessStatusCode)
            {
                return new ResponseTransacciones
                {
                    IsSuccess = false,
                    Message = "Error al comunicarse con el servicio"
                };
            }

            return await response.Content
                .ReadFromJsonAsync<ResponseTransacciones>();
        }

    }
}
