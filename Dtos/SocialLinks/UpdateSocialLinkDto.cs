using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BagApi.Dtos.SocialLinks;

public record class UpdateSocialLinkDto
(
    string Name,
    [Url] string Link
);