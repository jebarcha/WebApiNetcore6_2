namespace API.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime Creation { get; set; }
    public BrandDto Brand { get; set; }
    public CategoryDto Category { get; set; }
}

