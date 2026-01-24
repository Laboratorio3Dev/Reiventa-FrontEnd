using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System.Net.Http.Headers;
using WebBackOffice.DTO.BackOffice;
using WebBackOffice.Middleware;
using WebBackOffice.Pages.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpClient("ApiClient", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;

    if (string.IsNullOrWhiteSpace(settings.BaseUrl))
        throw new InvalidOperationException("ApiSettings:BaseUrl no configurado");

    client.BaseAddress = new Uri(settings.BaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<AuthenticationHandler>(); ;


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 200 * 1024 * 1024;
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/BackOffice/Login";
        options.AccessDeniedPath = "/BackOffice/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddRazorPages();
builder.Services.AddScoped<NPSService>();

builder.Services.AddScoped<UserSessionService>();

builder.Services.AddScoped<HoudiniServices>();
builder.Services.AddScoped<BackOfficeLabService>();
builder.Services.AddScoped<AdminHoudiniServices>();
builder.Services.AddScoped<RetencionServices>();
builder.Services.AddScoped<RetencionHipotecariaService>();

ExcelPackage.License.SetNonCommercialOrganization("WebBackOffice");
builder.Services.AddHttpContextAccessor();

// 2. Registrar el Handler
builder.Services.AddTransient<AuthenticationHandler>();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseSession();
// Registra tu Middleware personalizado AQUÍ
app.UseMiddleware<UnauthorizedMiddleware>();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
