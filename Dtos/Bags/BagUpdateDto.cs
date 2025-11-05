using System.ComponentModel.DataAnnotations;

namespace BagApi.Dtos.Bags;

public record class BagUpdateDto(
    [StringLength(5)] string Name,
    int BrandId,
    decimal Price
);
