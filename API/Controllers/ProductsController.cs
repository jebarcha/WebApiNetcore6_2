using API.Dtos;
using API.Helpers;
using API.Helpers.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;
[ApiVersion("1.0")]
[ApiVersion("1.1")]
[Authorize(Roles="Administrator")]

public class ProductsController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    //[HttpGet]
    //[SwaggerOperation(Summary = "Get products", Description = "Get all products")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<ActionResult<IEnumerable<ProductListDto>>> Get()
    //{
    //    var products = await _unitOfWork.Products.GetAllAsync();

    //    return _mapper.Map<List<ProductListDto>>(products);
    //}

    [HttpGet]
    [SwaggerOperation(Summary = "Get products", Description = "Get all products")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Pager<ProductListDto>>> Get([FromQuery] Params productParams)
    {
        var result = await _unitOfWork.Products.GetAllAsync(productParams.PageIndex, productParams.PageSize, productParams.Search);

        var listProductDto = _mapper.Map<List<ProductListDto>>(result.records);

        Response.Headers.Add("X-InlineCount", result.totalRecords.ToString());

        return new Pager<ProductListDto>(listProductDto, result.totalRecords, productParams.PageIndex, productParams.PageSize, productParams.Search);
    }

    [HttpGet]
    [MapToApiVersion("1.1")]
    [SwaggerOperation(Summary = "Get products", Description = "Get all products")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Get11()
    {
        var products = await _unitOfWork.Products.GetAllAsync();

        return _mapper.Map<List<ProductDto>>(products);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get product by Id", Description = "Get a specific product by Id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
            return NotFound(new ApiResponse(404, "Requested product does not exist"));
            //throw new Helpers.Errors.ApplicationExceptions.NotFoundException(nameof(Get), id);

        return _mapper.Map<ProductDto>(product);
    }

    //POST: api/Products
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new product", Description = "Add a new producct sending product object")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> Post(ProductAddUpdateDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        _unitOfWork.Products.Add(product);
        await _unitOfWork.SaveAsync();
        if (product == null)
        {
            return BadRequest(new ApiResponse(400));
        }
        productDto.Id = product.Id;
        return CreatedAtAction(nameof(Post), new { id = productDto.Id }, productDto);
    }

    //PUT: api/products/4
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Modify an existing product", Description = "Modify existing producct sending product Id and product object")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductAddUpdateDto>> Put(int id, [FromBody] ProductAddUpdateDto productDto)
    {
        if (productDto is null)
            return NotFound(new ApiResponse(404, "Requested product does not exist"));
            //throw new Helpers.Errors.ApplicationExceptions.NotFoundException(nameof(Put), id);

        var productDb = await _unitOfWork.Products.GetByIdAsync(id);
        if (productDb is null)
            return NotFound(new ApiResponse(404, "Requested product does not exist"));
            //throw new Helpers.Errors.ApplicationExceptions.NotFoundException(nameof(Put), id);

        var product = _mapper.Map<Product>(productDto);
        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveAsync();

        return productDto;
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a product", Description = "Delete an existing producct")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
            return NotFound(new ApiResponse(404, "Requested product does not exist"));
            //throw new Helpers.Errors.ApplicationExceptions.NotFoundException(nameof(Put), id);

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveAsync();

        return NoContent();
    }
}
