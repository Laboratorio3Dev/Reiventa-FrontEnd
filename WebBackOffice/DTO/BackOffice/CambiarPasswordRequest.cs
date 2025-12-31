namespace WebBackOffice.DTO.BackOffice
{
    public class CambiarPasswordRequest
    {
        public string? Usuario { get; set; }
        public string Password_Old { get; set; }
        public string Password_New { get; set; }
    }
}
