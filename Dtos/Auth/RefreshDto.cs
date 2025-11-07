namespace BagApi.Dtos.Auth;

public record class RefreshDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
