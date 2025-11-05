using System.ComponentModel.DataAnnotations;

namespace BagApi.Dtos.Bags;

public record class BagCreateDto
(
    [Required][StringLength(5)] string Name,
    [Required] int BrandId,
    [Required] decimal Price
);