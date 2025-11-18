namespace BagApi.Entities;

public class Shoes
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int BrandId { get; set; }

    public Brand? Brand { get; set; }

    public decimal Price { get; set; }

    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
