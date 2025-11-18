using BagApi.Dtos.Shoes;
using BagApi.Entities;

namespace BagApi.Mapping;

public static class ShoesMapping
{
    public static Shoes ToEntity(this CreateShoesDto shoes)
    {
        return new Shoes()
        {
            Name = shoes.Name,
            BrandId = shoes.BrandId,
            Price = shoes.Price
        };
    }

    public static Shoes ToEntity(this UpdateShoesDto shoes, int Id)
    {
        return new Shoes()
        {
            Id = Id,
            Name = shoes.Name,
            BrandId = shoes.BrandId,
            Price = shoes.Price
        };
    }

    public static ShoesDetailDto ToShoesDetailDto(this Shoes shoes)
    {
        return new ShoesDetailDto(
            shoes.Id,
            shoes.Name,
            shoes.Brand!,
            shoes.Price,
            shoes.CreatedAt
        );
    }
}