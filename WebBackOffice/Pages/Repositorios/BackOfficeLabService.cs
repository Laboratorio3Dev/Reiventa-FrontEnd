


using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using WebBackOffice.DTO.Aprendizaje;
using WebBackOffice.DTO.BackOffice;

namespace WebBackOffice.Pages.Repositorios
{
    public class BackOfficeLabService
    {
        private readonly HttpClient _http;

        public BackOfficeLabService(HttpClient http)
        {
            _http = http;
        }

        public async Task<LoginResponse> LoginAsync(string Usuario, string Password)
        {
            LoginRequest request = new LoginRequest();
            request.Usuario = Usuario;
            request.Password = Password;
            var response = await _http.PostAsJsonAsync("api/BackOffice/Login/acceso", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                return result!;
            }

            return new LoginResponse
            {
                IsSuccess = false,
                ErrorMessage = "Usuario o contraseña incorrectos"
            };
        }

        public async Task<List<PlanAccionDTO>> PlanesAccion(string token, string Usuario)
        {
            _http.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue("Bearer", token);

            UsuarioLogeado usuarioLogeado = new UsuarioLogeado();
            usuarioLogeado.Usuario = Usuario;

            var response = await _http.PostAsJsonAsync("api/BackOffice/Aprendizaje/PlanesAccion", usuarioLogeado);


            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<PlanAccionDTO>>();
            }

            return null;
        }

        public async Task<ResponseTransacciones?> IniciarPlan(string token, InicioPlanDTO request)
        {
            _http.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsJsonAsync("api/BackOffice/Aprendizaje/IniciarPlan", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseTransacciones>();
            }

            return null;
        }

        public async Task<ResponseTransacciones?> EnviarGO(string token, EnvioGO_DTO request)
        {
            _http.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsJsonAsync("api/BackOffice/Aprendizaje/EnviarGO", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseTransacciones>();
            }

            return null;
        }

        public async Task<ResponseTransacciones?> RechazoGO(string token, RechazoGO_DTO request)
        {
            _http.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsJsonAsync("api/BackOffice/Aprendizaje/RechazoGO", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseTransacciones>();
            }

            return null;
        }

        public async Task<ResponseTransacciones?> CompletarPlanGO(string token, CompletarGO_DTO request)
        {
            _http.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsJsonAsync("api/BackOffice/Aprendizaje/CompletarPlanGO", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseTransacciones>();
            }

            return null;
        }

        public async Task<DatosGeneralesDTO?> CargarCombosGenerales(string token, string Usuario)
        {
            _http.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue("Bearer", token);

            UsuarioLogeado usuarioLogeado = new UsuarioLogeado();
            usuarioLogeado.Usuario = Usuario;

            var response = await _http.PostAsJsonAsync("api/BackOffice/Aprendizaje/CombosGenerales", usuarioLogeado);


            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DatosGeneralesDTO>();
            }

            return null;
        }

        public async Task<List<TareasDTO>> Tareas(string token, int? Producto, int? Dimension)
        {
            _http.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue("Bearer", token);

            BusquedaTareas busquedaTareas = new BusquedaTareas();
            busquedaTareas.Producto = Producto;
            busquedaTareas.Dimension = Dimension;

            var response = await _http.PostAsJsonAsync("api/BackOffice/Aprendizaje/Tareas", busquedaTareas);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TareasDTO>>();
            }

            return null;
        }

        public async Task<ResponseTransacciones?> AsignarTareas(string token, List<CreaTareaDTO> creaTareaDTOs)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                creaTareaDTOs = creaTareaDTOs
            };

            var response = await _http.PostAsJsonAsync(
                "api/BackOffice/Aprendizaje/AsignarTareas",
                request
            );

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseTransacciones>();
            }

            return null;
        }

        public async Task<List<ProductoInsightDTO>> ListarComentarios(string token, RequestComentarios request)
        {
            _http.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.PostAsJsonAsync("api/BackOffice/Aprendizaje/ListarComentarios", request);


            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProductoInsightDTO>>();
            }

            return null;
        }

        public async Task<List<ListadoDashboard_DTO>> DatosDashboard(string token, RequestDashBoard request)
        {
            _http.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.PostAsJsonAsync("api/BackOffice/Aprendizaje/DatosDashboard", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ListadoDashboard_DTO>>();
            }
            return null;
        }

        public async Task<ResponseTransacciones?> CargarExcel(
                string token,
                IFormFile file,
                string usuario)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();

            // ARCHIVO
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "file", file.FileName);

            // USUARIO
            content.Add(new StringContent(usuario), "usuario");

            var response = await _http.PostAsync(
                "api/BackOffice/Aprendizaje/CargarDataDashoard",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                return await response.Content
                    .ReadFromJsonAsync<ResponseTransacciones>();
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine(error);

            return null;
        }

    }
}
