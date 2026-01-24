using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;

namespace WebBackOffice.Middleware
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NavigationManager _navigationManager;
        public AuthenticationHandler(IHttpContextAccessor httpContextAccessor, NavigationManager navigationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Se ejecuta la petición hacia el API
            var response = await base.SendAsync(request, cancellationToken);

            // 2. Si el servidor responde 401 (Unauthorized)
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Opcional: Aquí podrías borrar el token del LocalStorage 
                // para que no intente usar el expirado de nuevo.

                // 3. Redirigir al usuario

                _navigationManager.NavigateTo("/BackOffice/Login", forceLoad: true);
            }

            return response;
        }
    }
}
