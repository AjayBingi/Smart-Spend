using SmartSpendService.Models;
using SmartSpendService.Repositories;

namespace SmartSpendService.Services;

public class CategoryService
{
    private readonly CategoryLibrary _categoryLibrary;

    public CategoryService(CategoryLibrary categoryLibrary)
    {
        _categoryLibrary = categoryLibrary;
    }

    public List<Category> GetAll()
        => _categoryLibrary.GetAll();

    public Category? GetById(int id)
        => _categoryLibrary.GetById(id);
}