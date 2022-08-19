using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class ProductAddUpdateDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "The name of the product is mandatory")]
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime Creation { get; set; }
    public int BrandId { get; set; }
    public int CategoryId { get; set; }
}

