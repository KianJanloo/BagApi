namespace BagApi.Dtos.Bags;

public record class BagDto(
    int Id,
    string Name,
    int BrandId,
    decimal Price,
    DateOnly CreatedAt
);
