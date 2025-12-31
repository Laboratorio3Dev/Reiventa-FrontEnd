namespace WebBackOffice.DTO.BackOffice
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Icono { get; set; }
        public string Url { get; set; }
        public List<MenuItemDto> Hijos { get; set; } = new();
    }
}
