namespace WebBackOffice.DTO.BackOffice
{
    public class UserSessionService
    {
        public string? Usuario { get; set; }
        public string? Nombre { get; set; }
        public string? Token { get; set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
        public ICollection<Menu> MenuUsuario { get; set; } = new List<Menu>();
        public ICollection<Rol> RolesUsuario { get; set; } = new List<Rol>();

        public void Clear()
        {
            Usuario = null;
            Nombre = null;
            Token = null;
            MenuUsuario.Clear();
            RolesUsuario.Clear();
        }
    }
}
