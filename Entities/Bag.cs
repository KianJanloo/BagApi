using System;

namespace BagApi.Entities;

public class Bag
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int BrandId { get; set; }

    public Brand? Brand { get; set; }

    public decimal Price { get; set; }

    public DateOnly CreatedAt { get; set; }
}
