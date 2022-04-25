namespace Caketime.Repositories;

public interface IRecipeRepository
{
    Task<Recipe> AddRecipe(Recipe newRecipe);
    Task<List<Recipe>> GetRecipes();
    Task DeleteRecipe(string id);
}

public class RecipeRepository : IRecipeRepository
{
    private readonly IMongoContext _context;

    public RecipeRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<Recipe>> GetRecipes() => await _context.RecipeCollection.Find(_ => true).ToListAsync();

    public async Task<Recipe> GetRecipeById(string id) => await _context.RecipeCollection.Find(r => r.RecipeId == id).FirstOrDefaultAsync();
    public async Task<Recipe> AddRecipe(Recipe newRecipe)
    {
        await _context.RecipeCollection.InsertOneAsync(newRecipe);
        return newRecipe;
    }
    public async Task DeleteRecipe(string id) => await _context.RecipeCollection.DeleteOneAsync(r => r.RecipeId == id);
}