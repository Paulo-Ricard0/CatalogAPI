using Asp.Versioning;
using AutoMapper;
using CatalogAPI.DTOs;
using CatalogAPI.Models;
using CatalogAPI.Pagination;
using CatalogAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using X.PagedList;

namespace CatalogAPI.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _repository;
    private readonly IMapper _mapper;
    public ProductsController(IUnitOfWork repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [Authorize(Policy = "User")]
    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get([FromQuery] ProductsParameters productsParameters)
    {
        var products = await _repository.ProductRepository.GetProductsAsync(productsParameters);

        return GetProducts(products);
    }

    [Authorize(Policy = "User")]
    [HttpGet("filter/price/pagination")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsFilterPrice([FromQuery] ProductFilterPrice productFilterParams)
    {
        var products = await _repository.ProductRepository.GetProductsFilterPriceAsync(productFilterParams);

        return GetProducts(products);
    }

    private ActionResult<IEnumerable<ProductDTO>> GetProducts(IPagedList<Product> products)
    {
        var metadata = new
        {
            products.Count,
            products.PageSize,
            products.PageCount,
            products.TotalItemCount,
            products.HasNextPage,
            products.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [Authorize(Policy = "Admin")]
    [HttpPatch("{id:int}/UpdatePartial")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProductDTOUpdateResponse>> Patch(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDTO)
    {
        if (patchProductDTO == null || id <= 0)
        {
            return BadRequest();
        }

        var product = await _repository.ProductRepository.GetAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound("Produto não encontrado.");
        }

        var productUpdateRequest = _mapper.Map<ProductDTOUpdateRequest>(product);

        patchProductDTO.ApplyTo(productUpdateRequest, ModelState);

        if (!ModelState.IsValid || TryValidateModel(productUpdateRequest))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(productUpdateRequest, product);

        _repository.ProductRepository.Update(product);
        await _repository.CommitAsync();

        var productDTOUpdatedResponse = _mapper.Map<ProductDTOUpdateResponse>(product);
        return Ok(productDTOUpdatedResponse);
    }

    [Authorize(Policy = "User")]
    [HttpGet("Category/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategory(int id)
    {
        var products = await _repository.ProductRepository.GetProductsByCategoriesAsync(id);

        if (products.IsNullOrEmpty())
        {
            return NotFound("Produto não encontrado");
        }

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [Authorize(Policy = "User")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
    {
        var products = await _repository.ProductRepository.GetAllAsync();

        if (products == null)
        {
            return NotFound("Produtos não encontrados.");
        }

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [Authorize(Policy = "User")]
    [HttpGet("{id:int}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProductDTO>> GetById(int? id)
    {
        if (id == null || id <= 0)
        {
            return BadRequest("Id de produto inválido.");
        }

        var product = await _repository.ProductRepository.GetAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound("Produto não encontrado...");
        }

        var productDTO = _mapper.Map<ProductDTO>(product);

        return Ok(productDTO);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProductDTO>> Post(ProductDTO productDTO)
    {
        if (productDTO == null)
        {
            return BadRequest();
        }

        var product = _mapper.Map<Product>(productDTO);

        var newProduct = _repository.ProductRepository.Create(product);
        await _repository.CommitAsync();

        var newProductDTO = _mapper.Map<ProductDTO>(newProduct);

        return new CreatedAtRouteResult("GetProduct", new { id = newProductDTO.Id }, newProductDTO);
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProductDTO>> Put(int id, ProductDTO productDTO)
    {
        if (id != productDTO.Id)
        {
            return BadRequest();
        }

        var product = _mapper.Map<Product>(productDTO);

        var productUpdated = _repository.ProductRepository.Update(product);
        await _repository.CommitAsync();

        var productUpdatedDTO = _mapper.Map<ProductDTO>(productUpdated);
        return Ok(productUpdatedDTO);
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProductDTO>> Delete(int id)
    {
        var product = await _repository.ProductRepository.GetAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound("Produto não encontrado.");
        }

        var productDeleted = _repository.ProductRepository.Delete(product);
        await _repository.CommitAsync();

        var productDeletedDTO = _mapper.Map<ProductDTO>(productDeleted);
        return Ok(productDeletedDTO);
    }
}
