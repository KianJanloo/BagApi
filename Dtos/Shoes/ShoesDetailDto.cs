using BagApi.Entities;

namespace BagApi.Dtos.Shoes;

public record class ShoesDetailDto(
    int Id,
    string Name,
    Brand Brand,
    decimal Price,
    DateOnly CreatedAt
);
