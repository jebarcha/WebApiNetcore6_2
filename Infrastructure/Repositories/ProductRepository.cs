using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(MyLittleStoreContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsMoreExpensive(int cantidad) =>
                    await _context.Products
                        .OrderByDescending(p => p.Price)
                        .Take(cantidad)
                        .ToListAsync();

    public override async Task<Product> GetByIdAsync(int id, bool noTracking = true)
    {
        var queryProduct = noTracking
            ? _context.Products.AsNoTracking()
            : _context.Products;

        return await queryProduct
                        .Include(p => p.Brand)
                        .Include(p => p.Category)
                        .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Product>> GetAllAsync(bool noTracking = true)
    {
        var queryProduct = noTracking
            ? _context.Products.AsNoTracking()
            : _context.Products;

        return await queryProduct
            .Include(u => u.Brand)
            .Include(u => u.Category)
            .ToListAsync();
    }

    public override async Task<(int totalRecords, IEnumerable<Product> records)> GetAllAsync(int pageIndex, int pageSize, string search, bool noTracking = true)
    {
        var queryProduct = noTracking
            ? _context.Products.AsNoTracking()
            : _context.Products;

        if (!String.IsNullOrEmpty(search))
        {
            queryProduct = queryProduct.Where(p => p.Name.ToLower().Contains(search));
        }

        var totalRegistros = await queryProduct
                                    .CountAsync();

        var registros = await queryProduct
                                .Include(u => u.Brand)
                                .Include(u => u.Category)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return (totalRegistros, registros);
    }


}

