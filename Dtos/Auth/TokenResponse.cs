namespace BagApi.Dtos.Auth;

public record class TokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
