namespace BagApi.Dtos.Auth;

public record class RegisterDto
{
    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}
