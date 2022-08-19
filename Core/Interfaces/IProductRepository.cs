using Core.Entities;
namespace Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsMoreExpensive(int cantidad);
}


