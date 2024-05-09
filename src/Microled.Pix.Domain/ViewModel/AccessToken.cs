namespace Microled.Pix.Domain.ViewModel
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public bool active { get; set; }
        public DateTime ObtainedAt { get; set; } = DateTime.UtcNow;

    }
}
