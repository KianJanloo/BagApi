using System.ComponentModel.DataAnnotations;

namespace BagApi.Dtos.SocialLinks;

public record class CreateSocialLinkDto
(
    [Required] string Name,
    [Required][Url] string Link
);
