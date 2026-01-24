namespace WebBackOffice.Middleware
{
    public class UnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Deja que la petición siga su curso hacia el API
            await _next(context);

            // Al regresar, si el API respondió con 401:
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                // 1. Limpiamos la sesión para borrar el token inválido
                context.Session.Clear();

                // 2. Redirigimos al Login con un parámetro para avisar al usuario
                context.Response.Redirect("/BackOffice/Login");
            }
        }
    }
}
