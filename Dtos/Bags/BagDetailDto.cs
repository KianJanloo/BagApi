using BagApi.Entities;

namespace BagApi.Dtos.Bags;

public record class BagDetailDto(
    int Id,
    string Name,
    Brand Brand,
    decimal Price,
    DateOnly CreatedAt
);
