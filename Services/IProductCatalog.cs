using ProductCatalog.Models;

namespace ProductCatalog.Services;

public interface IProductCatalog
{
    IReadOnlyList<Product> GetAll();
    Product? GetById(int id);
    Product Add(Product product);
}
