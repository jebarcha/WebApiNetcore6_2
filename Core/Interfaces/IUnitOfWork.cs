namespace Core.Interfaces;
public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IBrandRepository Brands { get; }
    ICategoryRepository Categories { get; }
    IRoleRepository Roles { get; }
    IUserRepository Users { get; }
    Task<int> SaveAsync();
}

