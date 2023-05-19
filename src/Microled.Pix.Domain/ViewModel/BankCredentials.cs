namespace Microled.Pix.Domain.ViewModel
{
    public class BankCredentials
    {
        public BankCredentials(string clientId, string clientSecret, string token)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Token = token;
        }
        public BankCredentials()
        {
                    
        }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Token { get; set; }

    }
}
