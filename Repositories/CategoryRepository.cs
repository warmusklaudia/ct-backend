namespace Caketime.Repositories;

public interface ICategoryRepository
{
    Task<Category> AddCategory(Category newCategory);
    Task<List<Category>> GetCategories();
}

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoContext _context;

    public CategoryRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCategories() => await _context.CategoryCollection.Find(_ => true).ToListAsync();

    public async Task<Category> AddCategory(Category newCategory)
    {
        await _context.CategoryCollection.InsertOneAsync(newCategory);
        return newCategory;
    }
}