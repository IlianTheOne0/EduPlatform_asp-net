namespace TestingPlatform.Features.Categories.Services;

using TestingPlatform.Data;
using TestingPlatform.Features.Categories.Interfaces;
using TestingPlatform.Features.Tests.Models;

using Microsoft.EntityFrameworkCore;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context) => _context = context;

    public async Task<List<Category>> GetAllCategoriesAsync() =>
        await _context.Categories.Include(category => category.Tests)
        .OrderBy(category => category.Name)
        .ToListAsync();

    public async Task<Category?> GetCategoryByIdAsync(int id) =>
        await _context.Categories.Include(category => category.Tests)
        .FirstOrDefaultAsync(category => category.Id == id);

    public async Task<Category> CreateCategoryAsync(string name)
    {
        var category = new Category { Name = name };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await GetCategoryByIdAsync(id);
        if (category == null) { return false; }

        var testIds = category.Tests.Select(test => test.Id).ToList();
        if (testIds.Any())
        {
            var answersToDelete = await _context.AttemptAnswers
                .Where(answer => testIds.Contains(answer.Question!.TestId))
                .ToListAsync();

            _context.AttemptAnswers.RemoveRange(answersToDelete);
            _context.Tests.RemoveRange(category.Tests);
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return true;
    }
}