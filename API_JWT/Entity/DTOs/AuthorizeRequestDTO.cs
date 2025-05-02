namespace Entity.DTOs
{
    public class AuthorizeRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ResponseType { get; set; } = "code";
        public string ClientId { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
    }
}
