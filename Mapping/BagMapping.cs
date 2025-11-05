using BagApi.Dtos.Bags;
using BagApi.Entities;

namespace BagApi.Mapping;

public static class BagMapping
{

    public static Bag ToEntity(this BagCreateDto bag)
    {
        return new Bag()
        {
            Name = bag.Name,
            BrandId = bag.BrandId,
            Price = bag.Price
        };
    }

    public static Bag ToEntity(this BagUpdateDto bag, int Id)
    {
        return new Bag()
        {
            Id = Id,
            Name = bag.Name,
            BrandId = bag.BrandId,
            Price = bag.Price
        };
    }

    public static BagDetailDto ToBagDto(this Bag bag)
    {
        return new(
            bag.Id,
            bag.Name,
            bag.Brand!,
            bag.Price,
            bag.CreatedAt
        );
    }

}
