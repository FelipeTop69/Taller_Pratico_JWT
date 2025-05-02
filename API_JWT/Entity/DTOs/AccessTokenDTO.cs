namespace Entity.DTOs
{
    public class AccessTokenDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
