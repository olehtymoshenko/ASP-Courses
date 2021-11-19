namespace Meets.WebApi.Features.User.Helpers;

public class TokenPair
{
    public string AccessToken { get; }
    public string RefreshToken { get; }

    public TokenPair(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}
