namespace BagApi.Dtos.SocialLinks;

public record class SocialLinkDto(
    int Id,
    string Name,
    string Link,
    DateTime CreatedAt
);