namespace Caketime.Repositories;

public interface ICategoryRepository
{
    Task<Category> AddCategory(Category newCategory);
    Task<List<Category>> GetCategories();
    Task<Category> GetCategoryByName(string name);
    Task<Category> GetCategoryById(string id);
    Task<Category> UpdateCategory(string id, string name);

}

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoContext _context;

    public CategoryRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCategories() => await _context.CategoryCollection.Find(_ => true).ToListAsync();
    public async Task<Category> GetCategoryByName(string name) => await _context.CategoryCollection.Find<Category>(c => c.Name == name).FirstOrDefaultAsync();
    public async Task<Category> GetCategoryById(string id) => await _context.CategoryCollection.Find<Category>(c => c.Id == id).FirstOrDefaultAsync();
    public async Task<Category> AddCategory(Category newCategory)
    {
        await _context.CategoryCollection.InsertOneAsync(newCategory);
        return newCategory;
    }

    public async Task<Category> UpdateCategory(string id, string name)
    {
        try
        {
            var filter = Builders<Category>.Filter.Eq("Id", id);
            var update = Builders<Category>.Update.Set("Name", name);
            var result = await _context.CategoryCollection.UpdateOneAsync(filter, update);
            return await GetCategoryById(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}