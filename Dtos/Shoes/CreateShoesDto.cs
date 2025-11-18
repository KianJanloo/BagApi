using System.ComponentModel.DataAnnotations;

namespace BagApi.Dtos.Shoes;

public record class CreateShoesDto(
    [Required] string Name,
    [Required] int BrandId,
    [Required] decimal Price
);