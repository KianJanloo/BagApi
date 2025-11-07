using System;
using BagApi.Dtos.SocialLinks;
using BagApi.Entities;

namespace BagApi.Mapping;

public static class SocialLinkMapping
{
    public static SocialLink ToEntity(this CreateSocialLinkDto link)
    {
        return new SocialLink()
        {
            Name = link.Name,
            Link = link.Link
        };
    }

    public static SocialLinkDto ToSocialLinkDto(this SocialLink link)
    {
        return new(
            link.Id,
            link.Name,
            link.Link,
            link.CreatedAt
        );
    }
}
