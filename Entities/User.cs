using System;

namespace BagApi.Entities;

public class User
{
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
