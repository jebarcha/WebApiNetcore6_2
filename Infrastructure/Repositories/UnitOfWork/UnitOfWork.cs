using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly MyLittleStoreContext _context;
    private IProductRepository _products;
    private IBrandRepository _brands;
    private ICategoryRepository _categorias;

    public UnitOfWork(MyLittleStoreContext context)
    {
        _context = context;
    }

    public ICategoryRepository Categories => _categorias ??= new CategoryRepository(_context);

    public IBrandRepository Brands => _brands ??= new BrandRepository(_context);

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
    
}

