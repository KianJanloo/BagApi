using BagApi.Entities;

namespace BagApi.Dtos.Shoes;

public record class ShoesDto(
    int Id,
    string Name,
    int BrandId,
    decimal Price,
    DateOnly CreatedAt
);