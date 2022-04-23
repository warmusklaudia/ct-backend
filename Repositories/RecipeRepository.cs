namespace Caketime.Repositories;

public interface IRecipeRepository
{
    Task<Recipe> AddRecipe(Recipe newRecipe);
    Task<List<Recipe>> GetRecipes();
}

public class RecipeRepository : IRecipeRepository
{
    private readonly IMongoContext _context;

    public RecipeRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<Recipe>> GetRecipes() => await _context.RecipeCollection.Find(_ => true).ToListAsync();

    public async Task<Recipe> AddRecipe(Recipe newRecipe)
    {
        await _context.RecipeCollection.InsertOneAsync(newRecipe);
        return newRecipe;
    }
}