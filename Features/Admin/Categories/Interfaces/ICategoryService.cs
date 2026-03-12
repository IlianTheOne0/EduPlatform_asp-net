namespace TestingPlatform.Features.Categories.Interfaces;

using TestingPlatform.Features.Tests.Models;

public interface ICategoryService
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(string name);
    Task<bool> DeleteCategoryAsync(int id);
}