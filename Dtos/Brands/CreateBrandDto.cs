using System.ComponentModel.DataAnnotations;

namespace BagApi.Dtos.Brands;

public record class CreateBrandDto
(
    [Required] string Name
);