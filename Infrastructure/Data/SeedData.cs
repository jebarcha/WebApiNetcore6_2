using Core.Entities;
using CsvHelper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class SeedData
    {
        public static async Task SeedAsync(MyLittleStoreContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if (!context.Brands.Any())
                {
                    using (var readerBrands = new StreamReader(path + @"/Data/Csvs/marcas.csv"))
                    {
                        using (var csvBrands = new CsvReader(readerBrands, CultureInfo.InvariantCulture))
                        {
                            var brands = csvBrands.GetRecords<Brand>();
                            context.Brands.AddRange(brands);
                            await context.SaveChangesAsync();
                        }
                    }
                }

                if (!context.Categories.Any())
                {
                    using (var readerCategorias = new StreamReader(path + @"/Data/Csvs/categorias.csv"))
                    {
                        using (var csvCategories = new CsvReader(readerCategorias, CultureInfo.InvariantCulture))
                        {
                            var categorias = csvCategories.GetRecords<Category>();
                            context.Categories.AddRange(categorias);
                            await context.SaveChangesAsync();
                        }
                    }
                }

                if (!context.Products.Any())
                {
                    using (var readerProducts = new StreamReader(path + @"/Data/Csvs/productos.csv"))
                    {
                        using (var csvProducts = new CsvReader(readerProducts, CultureInfo.InvariantCulture))
                        {
                            var listProductsCsv = csvProducts.GetRecords<Product>();

                            List<Product> products = new List<Product>();
                            foreach (var item in listProductsCsv)
                            {
                                products.Add(new Product
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    Price = item.Price,
                                    Creation = item.Creation,
                                    CategoryId = item.CategoryId,
                                    BrandId = item.BrandId,
                                });
                            }

                            context.Products.AddRange(products);
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<SeedData>();
                logger.LogError(ex.Message);
            }
        }
    }
}
