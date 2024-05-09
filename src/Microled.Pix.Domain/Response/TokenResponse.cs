namespace Microled.Pix.Domain.Response
{
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }

        public static TokenResponse CreateNewToken(string token, int expires_in)
        {
            TokenResponse tokenResponse = new TokenResponse();
            tokenResponse.access_token = token;
            tokenResponse.expires_in = expires_in;

            return tokenResponse;
        }
    }
}
