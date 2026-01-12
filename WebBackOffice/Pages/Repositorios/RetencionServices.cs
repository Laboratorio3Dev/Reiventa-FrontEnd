using System.Net.Http.Headers;
using WebBackOffice.DTO.Common;


namespace WebBackOffice.Pages.Repositorios
{
    public class RetencionServices
    {
        private readonly HttpClient _http;

        public RetencionServices(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("ApiClient");
        }

        public async Task<ResponseTransacciones> CargarExcel(string token, IFormFile archivo)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();

            var streamContent = new StreamContent(archivo.OpenReadStream());
            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue(archivo.ContentType);

            content.Add(streamContent, "Archivo", archivo.FileName);

            var response = await _http.PostAsync(
                "api/Oficinas/Retencion/CargarExcel",
                content
            );
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                Console.WriteLine("❌ ERROR API VentaDigital");
                Console.WriteLine($"Status: {(int)response.StatusCode} - {response.StatusCode}");
                Console.WriteLine($"Body: {error}");

               
            }
  
            return await response.Content.ReadFromJsonAsync<ResponseTransacciones>();
        }
    }
}
