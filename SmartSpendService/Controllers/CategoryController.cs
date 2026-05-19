using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpendService.Services;

namespace SmartSpendService.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var categories = _categoryService.GetAll();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var category = _categoryService.GetById(id);
        if (category == null)
            return NotFound(new { message = "Category not found." });
        return Ok(category);
    }
}