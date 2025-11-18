namespace BagApi.Dtos.Shoes;

public record class UpdateShoesDto(
    string Name,
    int BrandId,
    decimal Price
);