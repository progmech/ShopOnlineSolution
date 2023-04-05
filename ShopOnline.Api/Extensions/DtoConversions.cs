using System.Linq;
using ShopOnline.Api.Entities;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Extensions
{
    public static class DtoConversions
    {
        public static IEnumerable<ProductDto> ConvertToDto(
            this IEnumerable<Product> products,
            IEnumerable<ProductCategory> productCategories)
        {
            return products.Join(productCategories, product => product.CategoryId, category => category.Id, (product, category) =>
            new ProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageURL = product.ImageURL,
                Price = product.Price,
                Qty = product.Qty,
                CategoryId = category.Id,
                CategoryName = category.Name
            }).ToList();
        }
    }
}