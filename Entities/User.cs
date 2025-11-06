using Microsoft.AspNetCore.Identity;

namespace BagApi.Entities
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
