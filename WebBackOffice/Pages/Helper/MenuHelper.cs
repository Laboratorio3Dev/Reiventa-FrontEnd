using WebBackOffice.DTO.BackOffice;

namespace WebBackOffice.Pages.Helper
{
    public static class MenuHelper
    {
        public static List<MenuItemDto> ConstruirMenu(List<Menu> menu)
        {
            var lookup = menu.ToDictionary(
                x => x.IdOpcion,
                x => new MenuItemDto
                {
                    Id = x.IdOpcion,
                    Titulo = x.Opcion,
                    Url = x.Url,
                    Icono = string.IsNullOrWhiteSpace(x.Icono)
                    ? "fa-solid fa-circle"
                    : $"fa-solid {x.Icono}"
                });

            var resultado = new List<MenuItemDto>();

            foreach (var item in menu.OrderBy(x => x.Orden))
            {
                if (item.IdPadre == 0)
                {
                    resultado.Add(lookup[item.IdOpcion]);
                }
                else if (lookup.ContainsKey(item.IdPadre))
                {
                    lookup[item.IdPadre].Hijos.Add(
                        lookup[item.IdOpcion]
                    );
                }
            }

            return resultado;
        }
    }

}
