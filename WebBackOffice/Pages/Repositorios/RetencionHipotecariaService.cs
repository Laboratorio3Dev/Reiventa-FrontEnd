using BootstrapBlazor.Components;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Net.Http.Headers;
using WebBackOffice.DTO.Common;
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

        public async Task<PagedResult<RetencionHipotecariaSolicitudDTO>> Listar(
      ListarRetencionHipotecariaRequest filtro,
      string token)
        {
            _http.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("Bearer", token);
            var query = new Dictionary<string, string?>()
            {
                ["Usuario"] = filtro.Usuario,
                ["FechaInicio"] = filtro.FechaInicio?.ToString("yyyy-MM-dd"),
                ["FechaFin"] = filtro.FechaFin?.ToString("yyyy-MM-dd"),
                ["Page"] = filtro.Page.ToString(),
                ["PageSize"] = filtro.PageSize.ToString()
            };

            var queryString = string.Join("&",
                query
                    .Where(x => !string.IsNullOrEmpty(x.Value))
                    .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}")
            );

            var url = $"api/Oficinas/RetencionHipotecaria?{queryString}";

            // 1. Usamos GetAsync para obtener la respuesta completa (status, headers, body)
            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // 2. Aquí es donde capturas lo que el Middleware del API escribió
                var errorContent = await response.Content.ReadAsStringAsync();

                // Esto saldrá en la consola del navegador/servidor
                System.Console.WriteLine($"❌ ERROR API: {response.StatusCode}");
                System.Console.WriteLine($"Detalle: {errorContent}");

                // Lanzamos una excepción para que el usuario sepa que falló
                throw new Exception($"Error en la API: {response.StatusCode}");
            }

            // 3. Si todo salió bien, desertializamos el JSON
            return await response.Content
       .ReadFromJsonAsync<PagedResult<RetencionHipotecariaSolicitudDTO>>()
       ?? new PagedResult<RetencionHipotecariaSolicitudDTO>();
        }



        public async Task<List<ProductoVincularDTO>> ListarProductosVincular( string token)
        {
            _http.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("Bearer", token);
            return await _http.GetFromJsonAsync<List<ProductoVincularDTO>>(
                "api/Oficinas/RetencionHipotecaria/productos"
            ) ?? new();
        }

        public async Task<List<EntidadesDTO>> ListarEntidades(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("Bearer", token);
            return await _http.GetFromJsonAsync<List<EntidadesDTO>>(
                "api/Oficinas/RetencionHipotecaria/Entidades"
            ) ?? new();
        }

        public async Task<ResponseTransacciones> GuardarSolicitud(
    RetencionHipotecariaGuardarDTO dto, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsJsonAsync(
                "api/Oficinas/RetencionHipotecaria/Crear",
                dto
            );

            if (!response.IsSuccessStatusCode)
            {

                var errorContent = await response.Content.ReadAsStringAsync();

                // Esto saldrá en la consola del navegador/servidor
                System.Console.WriteLine($"❌ ERROR API: {response.StatusCode}");
                System.Console.WriteLine($"Detalle: {errorContent}");
                return new ResponseTransacciones
                {
                    IsSuccess = false,
                    Message = "Error al guardar la solicitud"
                };
            }

            return await response.Content
                .ReadFromJsonAsync<ResponseTransacciones>()
                ?? new ResponseTransacciones
                {
                    IsSuccess = false,
                    Message = "Respuesta inválida del servidor"
                };
        }



        public async Task<PagedResult<ListadoHipotecarioDTO>> ListarNegocioHipotecario(
    ListarNegocioHipotecarioRequest filtro,
    string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var query = new Dictionary<string, string?>
            {
                ["FechaInicio"] = filtro.FechaInicio.ToString("yyyy-MM-dd"),
                ["FechaFin"] = filtro.FechaFin.ToString("yyyy-MM-dd"),
                ["Estado"] = filtro.Estado?.ToString(),
                ["Page"] = filtro.Page.ToString(),
                ["PageSize"] = filtro.PageSize.ToString()
            };

            var queryString = string.Join("&",
                query
                    .Where(x => !string.IsNullOrEmpty(x.Value))
                    .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}")
            );

            var url = $"api/Oficinas/NegocioHipotecario/Listar?{queryString}";

            var response = await _http.GetAsync(url);
         
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(errorContent);
            }

            return await response.Content
                .ReadFromJsonAsync<PagedResult<ListadoHipotecarioDTO>>()
                ?? new PagedResult<ListadoHipotecarioDTO>();
        }

        public async Task<List<SimulacionHipotecariaDTO>> ListarPorCliente(
       int codigoCliente,
       string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var url = $"api/Oficinas/NegocioHipotecario/simulaciones/{codigoCliente}";

            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error API: {response.StatusCode} - {error}");
            }

            return await response.Content
                .ReadFromJsonAsync<List<SimulacionHipotecariaDTO>>()
                ?? new List<SimulacionHipotecariaDTO>();
        }


        public async Task<ResponseTransacciones> ActualizarGestion(
  ActualizarGestionClienteRequest command,
  string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.PutAsJsonAsync(
                "api/Oficinas/NegocioHipotecario/gestion",
                command);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error actualizando gestión");

            return await response.Content
                .ReadFromJsonAsync<ResponseTransacciones>()
                ?? new ResponseTransacciones();
        }

    }
}
