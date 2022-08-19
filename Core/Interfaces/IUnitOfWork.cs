namespace Core.Interfaces;
public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IBrandRepository Brands { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveAsync();
}

