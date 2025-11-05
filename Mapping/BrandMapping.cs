using BagApi.Dtos.Brands;
using BagApi.Entities;

namespace BagApi.Mapping;

public static class BrandMapping
{

    public static Brand ToEntity(this CreateBrandDto brand)
    {
        return new Brand()
        {
            Name = brand.Name
        };
    }

    public static Brand ToEntity(this UpdateBrandDto brand, int Id)
    {
        return new Brand()
        {
            Id = Id,
            Name = brand.Name
        };
    }

    public static BrandDto ToBrandDto(this Brand brand)
    {
        return new(
            brand.Id,
            brand.Name
        );
    }

}
