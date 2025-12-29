namespace WebBackOffice.DTO.BackOffice
{
    public class Menu
    {
        public int IdOpcion { get; set; }
        public string Opcion { get; set; }
        public string Url { get; set; }
        public int IdPadre { get; set; }
        public int Orden { get; set; }
    }
}
