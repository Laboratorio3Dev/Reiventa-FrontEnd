using BootstrapBlazor.Components;
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

        public async Task<ResponseTransacciones> ObtenerProductoPorId(
    string token, int id)
        {
            _http.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetFromJsonAsync<ResponseTransacciones>(
                $"api/Oficinas/AdminHoudini/{id}"
            );

            return response!;
        }

        public async Task<ResponseTransacciones> ActualizarProducto(
    string token,
    ProductoDTO producto)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.PutAsJsonAsync(
                "api/Oficinas/AdminHoudini/Actualizar",
                producto
            );

            return await response.Content
                .ReadFromJsonAsync<ResponseTransacciones>();
        }



        public async Task<ResponseTransacciones> DesactivarProducto(
                string token,
                int idProducto)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.PatchAsync(
                $"api/Oficinas/AdminHoudini/Desactivar/{idProducto}",
                null
            );

            if (!response.IsSuccessStatusCode)
            {
                return new ResponseTransacciones
                {
                    IsSuccess = false,
                    Message = "Error al desactivar el producto"
                };
            }

            return await response.Content
                .ReadFromJsonAsync<ResponseTransacciones>();
        }
    }



}
