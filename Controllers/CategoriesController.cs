using Asp.Versioning;
using CatalogAPI.DTOs;
using CatalogAPI.DTOs.Mappings;
using CatalogAPI.Models;
using CatalogAPI.Pagination;
using CatalogAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;

namespace CatalogAPI.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _repository;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IUnitOfWork repository, ILogger<CategoriesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get([FromQuery] CategoriesParameters categoriesParams)
    {
        var categories = await _repository.CategoryRepository.GetCategoriesAsync(categoriesParams);

        return GetCategories(categories);
    }

    [HttpGet("filter/name/pagination")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetFilteredCategoriesAsync([FromQuery] CategoriesFilterName categoriesFilterParams)
    {
        var FilteredCategories = await _repository.CategoryRepository.GetCategoriesFilterNameAsync(categoriesFilterParams);
        return GetCategories(FilteredCategories);
    }

    private ActionResult<IEnumerable<CategoryDTO>> GetCategories(IPagedList<Category> categories)
    {
        var metadata = new
        {
            categories.Count,
            categories.PageSize,
            categories.PageCount,
            categories.TotalItemCount,
            categories.HasNextPage,
            categories.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var CategoriesDTO = categories.ToCategoryDTOList();
        return Ok(CategoriesDTO);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll()
    {
        var categories = await _repository.CategoryRepository.GetAllAsync();

        if (categories == null)
        {
            return NotFound("Categorias não encontradas.");
        }

        var CategoriesDTO = categories.ToCategoryDTOList();

        return Ok(CategoriesDTO);
    }

    [HttpGet("{id:int}", Name = "GetCategory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoryDTO>> GetById(int id)
    {
        var category = await _repository.CategoryRepository.GetAsync(c => c.Id == id);

        if (category == null)
        {
            _logger.LogWarning($"Categoria com id= {id} não encontrada...");
            return NotFound($"Categoria com id= {id} não encontrada...");
        }

        var categoryDTO = category.ToCategoryDTO();

        return Ok(categoryDTO);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoryDTO>> Post(CategoryDTO categoryDTO)
    {
        if (categoryDTO == null)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var category = categoryDTO.ToCategory();

        var categoryCreated = _repository.CategoryRepository.Create(category);
        await _repository.CommitAsync();

        var categoryCreatedDTO = categoryCreated.ToCategoryDTO();

        return new CreatedAtRouteResult("GetCategory", new { id = categoryCreatedDTO.Id }, categoryCreatedDTO);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoryDTO>> Put(int id, CategoryDTO categoryDTO)
    {
        if (id != categoryDTO.Id)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var category = categoryDTO.ToCategory();


        var categoryUpdated = _repository.CategoryRepository.Update(category);
        await _repository.CommitAsync();

        var categoryUpdatedDTO = categoryUpdated.ToCategoryDTO();

        return Ok(categoryUpdatedDTO);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoryDTO>> Delete(int id)
    {
        var category = await _repository.CategoryRepository.GetAsync(c => c.Id == id);

        if (category == null)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return NotFound($"Categoria com id={id} não encontrada...");
        }

        var categoryDeleted = _repository.CategoryRepository.Delete(category);
        await _repository.CommitAsync();

        var categoryDeletedDTO = categoryDeleted.ToCategoryDTO();

        return Ok(categoryDeletedDTO);
    }
}
