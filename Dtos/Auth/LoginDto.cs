namespace BagApi.Dtos.Auth;

public record class LoginDto
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
